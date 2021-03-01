package com.hw;

import android.app.Activity;
import android.content.Context;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.os.Handler;
//import android.support.annotation.NonNull;
//import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.widget.Toast;


import com.ss.union.game.sdk.LGSDK;
import com.ss.union.sdk.ad.LGAdManager;
import com.ss.union.sdk.ad.dto.LGBaseConfigAdDTO;
import com.ss.union.sdk.ad.dto.LGRewardVideoAdDTO;
import com.ss.union.sdk.ad.type.LGRewardVideoAd;
import com.unity3d.player.UnityPlayer;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Objects;
import java.util.Set;



import static android.Manifest.permission.ACCESS_COARSE_LOCATION;
import static android.Manifest.permission.WRITE_EXTERNAL_STORAGE;
import com.bytedance.applog.AppLog;
import com.bytedance.applog.InitConfig;
public class GamePlayer {

    private  String TAG = "GamePlayer";
    public Activity mContext;
    public static final String SAMPLE_VERTICAL_CODE_ID = "945637904";
    private LGAdManager lgADManager;
    private LGRewardVideoAd rewardVideoAd;
    private  int loadAderrorCount;
    private  int showAdCount;

    private  String AndroidPlatformWrapper = "AndroidPlatformWrapper";
    private  String rewardTag;
    private  String rewardActionSuccess;
    private  String rewardActionFail;
    private  String onLoadRewardResult = "OnLoadRewardResult";
    private  boolean isReward;
    public GamePlayer() {
    }

    public  void initHwSDK(Activity context, String url){
        mContext = context;
        Log.i(TAG, "initHwSDK: "+ Thread.currentThread().getId());
        context.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                //doyourcode
                Log.i(TAG, "initHwSDK: "+ Thread.currentThread().getId());
                // step1:LGADManager 广告管理类初始化
                lgADManager = LGSDK.getAdManager();
                // step2:(可选，强烈建议在合适的时机调用):申请部分权限，如read_phone_state,防止获取不了imei时候，下载类广告没有填充的问题。
                LGSDK.requestPermissionIfNecessary(mContext);
                loadAd(SAMPLE_VERTICAL_CODE_ID,LGRewardVideoAdDTO.ORIENTATION_VERTICAL);
            }});


        setInterListerner();
        setRewardListener();



    }
    /**
     * 加载广告
     *
     * @param codeId      广告ID
     * @param orientation 广告展示方向
     */
    private void loadAd(String codeId, int orientation) {
        //step3:创建广告请求参数AdSlot,具体参数含义参考文档
        LGRewardVideoAdDTO rewardVideoADDTO = new LGRewardVideoAdDTO();
        rewardVideoADDTO.context = mContext;
        // 广告位ID
        rewardVideoADDTO.codeID = codeId;
        // 唯一设备标识 用户ID
        rewardVideoADDTO.userID = "user123";
        // 期望返回的图片尺寸
        rewardVideoADDTO.expectedImageSize = new LGBaseConfigAdDTO.ExpectedImageSize(1080, 1920);
        // 奖励的名称
        rewardVideoADDTO.rewardName = "金币";
        // 奖励的数量
        rewardVideoADDTO.rewardAmount = 3;
        // 设置广告展示方向
        rewardVideoADDTO.videoPlayOrientation = orientation;

        //step4:请求广告
        lgADManager.loadRewardVideoAd(rewardVideoADDTO, new LGAdManager.RewardVideoAdListener() {
            @Override
            public void onError(int code, String message) {
                Log.e(TAG, "code:" + code + ",message:" + message);
                loadAderrorCount++;
                UnityPlayer.UnitySendMessage(AndroidPlatformWrapper,onLoadRewardResult,"false");
                new Handler().postDelayed(new Runnable() {
                    @Override
                    public void run() {
                        loadAd(codeId,orientation);
                    }
                },loadAderrorCount*5000);
            }

            @Override
            public void onRewardVideoAdLoad(LGRewardVideoAd ad) {
                Log.e(TAG, "onRewardVideoAdLoad");
                rewardVideoAd = ad;
                loadAderrorCount = 0;
                UnityPlayer.UnitySendMessage(AndroidPlatformWrapper,onLoadRewardResult,"true");
            }
        });
    }

    private void setRewardListener(){

    }

    private void setInterListerner(){

    }


    public void showHwRewardAd(String arg1,String arg2,String arg3){
        mContext.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                showAdCount++;
                if(showAdCount >1)return;
                rewardTag = arg1;
                Log.i(TAG, "showHwRewardAd: "+arg1 + "arg2:"+arg2);
                rewardActionSuccess = arg2;
                rewardActionFail = arg3;
                if (rewardVideoAd == null) {
                    Log.e(TAG, "请先加载广告");
                    return;
                }

//                mttRewardVideoAd.setShowDownLoadBar(false);
                // 设置用户操作交互回调，接入方可选择是否设置
                rewardVideoAd.setInteractionCallback(new LGRewardVideoAd.InteractionCallback() {

                    @Override
                    public void onAdShow() {
                        Log.e(TAG, "rewardVideoAd show");
                        showAdCount = 0;
                    }

                    @Override
                    public void onAdVideoBarClick() {
                        Log.e(TAG, "rewardVideoAd bar click");
                    }

                    @Override
                    public void onAdClose() {
                        Log.e(TAG, "rewardVideoAd close");
                        rewardVideoAd = null;
                        if(isReward)
                            UnityPlayer.UnitySendMessage(AndroidPlatformWrapper,rewardActionSuccess,rewardTag);
                        else
                            UnityPlayer.UnitySendMessage(AndroidPlatformWrapper,rewardActionFail,rewardTag);
                        isReward = false;
                        UnityPlayer.UnitySendMessage(AndroidPlatformWrapper,onLoadRewardResult,"false");
                        loadAd(SAMPLE_VERTICAL_CODE_ID,LGRewardVideoAdDTO.ORIENTATION_VERTICAL);
                    }

                    //视频播放完成回调
                    @Override
                    public void onVideoComplete() {

                        Log.e(TAG, "rewardVideoAd complete");
                        isReward = true;
                    }

                    @Override
                    public void onVideoError() {

                        Log.e(TAG, "rewardVideoAd error");
                        isReward = false;
                    }

                    //视频播放完成后，奖励验证回调，rewardVerify：是否有效，rewardAmount：奖励数量，rewardName：奖励名称
                    @Override
                    public void onRewardVerify(boolean rewardVerify, int rewardAmount, String rewardName) {
                        Log.e(TAG, "verify:" + rewardVerify + " amount:" + rewardAmount +
                                " name:" + rewardName);
                    }

                    @Override
                    public void onSkippedVideo() {
                        Log.e(TAG, "onSkippedVideo");

                    }
                });
                //step5  展示广告，并传入广告展示的场景
                rewardVideoAd.showRewardVideoAd(mContext);
            }});



    }
    public void showHwInterAd(){
        Log.i(TAG, "showHwInterAd: ");

    }
    public boolean isHwRewardLoaded(){
        Log.i(TAG, "isHwRewardLoaded: " );
        if (rewardVideoAd == null || showAdCount >=1) {
            Log.e(TAG, "广告还没准备好， 请先加载广告");
            return false;
        }
        return  true;
    }

    public  boolean isHwInterLoaded(){
        Log.i(TAG, "isHwInterLoaded: " );
        return  false;
    }
    public  void  OnApplicationPause(boolean pause){
        Log.i(TAG, "OnApplicationPause: " +pause);
    }


    public  void  TAEventPropertie(String custom,String dic){
        Log.i(TAG,"TAEventPropertie custom:"+custom+"=>dic:"+dic);
        String ssid = AppLog.getSsid(); // 获取数说 ID
        String did = AppLog.getDid(); // 获取服务端 device ID
        String iid = AppLog.getIid(); // 获取 install ID
        Log.i(TAG,"ssid:"+ssid+"did:"+did+"iid:"+iid);
        try{
            JSONObject paramsObj = new JSONObject(dic);
            Log.i(TAG,"TAEventPropertie paramsObj:"+paramsObj);
            AppLog.onEventV3(custom,paramsObj);

        }catch (Exception e){
            Log.i(TAG, "TAEventPropertie: " + e.toString());
            e.printStackTrace();
        }
    }


}
