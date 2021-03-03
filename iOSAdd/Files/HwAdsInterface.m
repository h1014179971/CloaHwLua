//
//  HwAdsInterface.m
//  Unity-iPhone
//
//  Created by game team on 2019/11/15.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <LightGameSDK/LGRewardedVideoAd.h>
#import <LightGameSDK/LightGameSDK.h>
#import <LightGameSDK/BDAutoTrackConfig.h>
#import <LightGameSDK/LightGameManager.h>
#import <Bugly/Bugly.h>

#import "HwAdsInterface.h"
//@interface HwAdsInterface()
//
//@property(nonatomic,strong)LGRewardedVideoAd *rewardedVideoAd;
//
//@end

@implementation HwAdsInterface
typedef void (*CallbackDelegate)(const char *object);
CallbackDelegate callback;


static HwAdsInterface *hwAdsInterfaceInstance;
+ (id) sharedInstance{
    if(hwAdsInterfaceInstance == nil){
        NSLog(@"shareInstance");
        hwAdsInterfaceInstance = [[self alloc] init];
    }
    return hwAdsInterfaceInstance;
}
/*
 视频加载成功
 */
-(void)rewardedVideoAdDidLoad:(LGRewardedVideoAd *)rewardedVideoAd{
    NSLog(@"rewardedVideoAdDidLoad ");
    self.isLoadRewardSuccess = true;
    UnitySendMessage("IOSPlatformWrapper","OnLoadRewardResult","true");
}
/*
 视频加载失败
 */
-(void)rewardedVideoAd: (LGRewardedVideoAd *)rewardedVideoAd didFailWithError:(nonnull NSError *)error{
    NSLog(@"rewardedVideoAd  didFailWithError");
    self.isLoadRewardSuccess = false;
    UnitySendMessage("IOSPlatformWrapper","OnLoadRewardResult","false");
    self.loadAderrorCount++;
    NSTimer *loadTimer = [NSTimer timerWithTimeInterval:self.loadAderrorCount *5 target:self selector:@selector(loadMHRewardAd) userInfo:nil repeats:NO];
                [[NSRunLoop currentRunLoop] addTimer:loadTimer forMode:NSDefaultRunLoopMode];
                [[NSRunLoop currentRunLoop] run];
}
/*
 此方法在成功缓存时被调用
 */
- (void)rewardedVideoAdVideoDidLoad:(LGRewardedVideoAd *)rewardedVideoAd{
    NSLog(@"rewardedVideoAdVideoDidLoad");
}
/*
 显示广告插槽调用
 */
- (void)rewardedVideoAdWillVisible:(LGRewardedVideoAd *)rewardedVideoAd{
    NSLog(@"rewardedVideoAdWillVisible");
}
/*
 当显示广告时调用
 */
- (void)rewardedVideoAdDidVisible:(LGRewardedVideoAd *)rewardedVideoAd{
    NSLog(@"rewardedVideoAdDidVisible");
}
/*
 即将关闭广告
 */
- (void)rewardedVideoAdWillClose:(LGRewardedVideoAd *)rewardedVideoAd{
    NSLog(@"rewardedVideoAdWillClose");
    self.isReward = true;
}
/*
 关闭广告
 */
- (void)rewardedVideoAdDidClose:(LGRewardedVideoAd *)rewardedVideoAd{
    NSLog(@"rewardedVideoAdDidClose");
    if(self.isReward){
        callback("true");
    }else{
        callback("false");
    }
    self.isReward = false;
    [self.rewardedVideoAd loadAdData];
}
/*
 广告点击
 */
- (void)rewardedVideoAdDidClick:(LGRewardedVideoAd *)rewardedVideoAd{
    NSLog(@"rewardedVideoAdDidClick");
}
/*
 播放失败
 */
- (void)rewardedVideoAdDidPlayFinish:(LGRewardedVideoAd *)rewardedVideoAd didFailWithError:(NSError *)error{
    NSLog(@"rewardedVideoAdDidPlayFinish%@",error);
    if(error != nil){
        if(callback != nil)
            callback("false");
        self.isReward = false;
        [self.rewardedVideoAd loadAdData];
    }
    
}
/*
 服务器验证成功 播放成功，发奖励
 */
- (void)rewardedVideoAdServerRewardDidSucceed:(LGRewardedVideoAd *)rewardedVideoAd verify:(BOOL)verify{
    NSLog(@"rewardedVideoAdServerRewardDidSucceed");
}
/*
 验证失败
 */
- (void)rewardedVideoAdServerRewardDidFail:(LGRewardedVideoAd *)rewardedVideoAd{
    NSLog(@"rewardedVideoAdServerRewardDidFail");
}
/*
 点击skip
 */
- (void)rewardedVideoAdDidClickSkip:(LGRewardedVideoAd *)rewardedVideoAd{
    NSLog(@"rewardedVideoAdDidClickSkip");
}







-(void)initMHSDK{
    NSLog(@"initHwSDK");
    [Bugly startWithAppId:@"4f9f219da7"];
    // 初始化 APM （APM 其他配置可参考 （16. APM崩溃检测），必须要接入）
        BDAutoTrackConfig *config = [BDAutoTrackConfig new];
        // 此处需要 CP 添加应用云 appid
        config.appID = @"209392";
        // 设置debug log 方便查找问题
        [LightGameManager isDebuLog:YES];
        // 设置debug log 为中文
        [LightGameManager debugType:LGDebugLogType_Chinese];
        
        // 深度转化相关功能配置
        LGBDConfig *bdCfg = [[LGBDConfig alloc] init];
        // 域名默认国内，新加坡:LGBDAutoTrackServiceVendorSG
        bdCfg.serviceVendor = LGBDAutoTrackServiceVendorCN;
        // 是否在控制台输出⽇日志，仅调试使⽤用。release版本请设置为 NO
        //bdCfg.showDebugLog = NO;
        // 是否加密⽇日志，默认加密。release版本请设置为 YES
        //bdCfg.logNeedEncrypt = YES;
        
        bdCfg.registerFinishBlock = ^(NSString * _Nullable deviceID, NSString * _Nullable installID, NSString * _Nullable ssID, NSString * _Nullable userUniqueID) {
            NSLog(@"设置一些依赖设备id的初始化工作");
        };
        
        [[LightGameManager sharedInstance] configTTTrack:bdCfg];
        
      
        /**
        * 帐号体系登录,如需接入请设置以下两项配置项
        **/
       
        // 是否开启帐号功能的Toast提示设置, 默认为yes, 可设置no关闭  非必须配置项
        //[LightGameLoginManager setIsLoginToastPrompt:YES];
        /**
         设置登录模式，分静默登录(LGLoginModeSilent)和弹框登录(LGLoginModePopView) 必须设置项!!!
         @param loginResult 登录结果
         @param loginUser 登录返回的用户信息
         @return nil
         */
        [LightGameLoginManager setLoginMode:LGLoginModeSilent];
        //设置隐私弹窗信息，依此传入主体名称，公司名称，更新日期，生效日期，注册地址（注册地址为营业执照注册地址,并且将获取权限以后才可以使用的初始化方法放到回调中，时间格式需为yyyy-MM-dd，否则会设置失败
        [[LightGameManager sharedInstance] setPrivacyPopupWithGameName:@"gameName" andComponyName:@"companyName" andUpdateDate:@"2020-05-06" andValidDate:@"2020-07-20" andRegisteredAddress:@"registeredAddress" complete:^{
      //此回调里是对推送功能和sdk进行初始化
          
            //是否开启推送功能
            // 沙盒测试 channel 传 sandbox；正式环境 channel 传 App Store
            [[LGPushManager sharedInstance] switchOpenPush:YES channel:@"sandbox"];
            [LightGameManager startWithAppID:@"5eQHjCwYKOWHKMdiW0Gn917fUXpRgdIHkTUiKN2VIclRbVKq4m3mUmVGtZEugyoJSaHqW8JuQVOg9lHna2NVq/dexUNwOd24DIg9CTTgvwyDHCww54AuuA8vQJfSQsZAaAr6sgy2Nh4W86squRswAcGnFpgsEDy39E7VelVW8qsYkAkgLN6VBZMPN3qX/lyKCHsD+bo46VVwC6XDa8NSeVv5/grEbB9YoaRV6mlmQbXhDJ4wMyjdgSTPWFs59qENmR8gDeySaJ4WeFfe0g/yrwGAByYH" appName:@"快递大亨_iOS" channel:@"App Store"];
          
          [LightGameManager setSDKInitCompletionHandler:^{
                // SDK 初始化完成,可在此处调用登录和广告等接口
                NSLog(@"SDK 初始化完成");
              LGRewardedVideoModel *rewardModel = [[LGRewardedVideoModel alloc] init];
              // 必填参数
              rewardModel.userId = @"945732907";
              self.rewardedVideoAd = [[LGRewardedVideoAd alloc] initWithSlotID:@"945732907" rewardedVideoModel:rewardModel];
              self.rewardedVideoAd.delegate = self;
              [self.rewardedVideoAd loadAdData];
           }];
        }];
    
    
}

-(void) loadMHInterAd{
    NSLog(@"call loadInterAd");
}


-(void) showMHInterAd{
    NSLog(@"call ShowInterAd");
}

-(BOOL) isMHInterAdLoaded{
    NSLog(@"call isInterLoaded");
	return NO;
}

-(void) loadMHRewardAd{
    NSLog(@"call loadRewardedVideo");
    [self.rewardedVideoAd loadAdData];
}

-(void) showMHRewardAd:(NSString *)tag{
    NSLog(@"call showRewardedVideo");
    self.rewardTag = tag;
    if (self.isLoadRewardSuccess && [self.rewardedVideoAd isAdValid]) {
        // 视频内容load成功后才能展示广告
        
        UIViewController *rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
        UIViewController *topVC = [self getCurrentVCFrom:rootVC];
        
        
//        UIViewController *appRootVC = [UIApplication sharedApplication].delegate.window.rootViewController;
//        UIViewController *topVC = appRootVC;
//                if(topVC.presentationController){
//                    topVC = topVC.presentationController;
//                }
        
//        UIViewController *appRootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
//        UIViewController *topVC = appRootVC;
        
//        if(topVC.presentationController){
//            topVC = topVC.presentationController;
//        }
        [self.rewardedVideoAd showAdFromRootViewController:topVC];
    }
}
- (UIViewController *)getCurrentVCFrom:(UIViewController *)rootVC{
    UIViewController *currentVC;
    if ([rootVC presentedViewController]) {
            // 视图是被presented出来的
            
            rootVC = [rootVC presentedViewController];
        }

        if ([rootVC isKindOfClass:[UITabBarController class]]) {
            // 根视图为UITabBarController
            
            currentVC = [self getCurrentVCFrom:[(UITabBarController *)rootVC selectedViewController]];
            
        } else if ([rootVC isKindOfClass:[UINavigationController class]]){
            // 根视图为UINavigationController
            
            currentVC = [self getCurrentVCFrom:[(UINavigationController *)rootVC visibleViewController]];
            
        } else {
            // 根视图为非导航类
            
            currentVC = rootVC;
        }
    return  currentVC;
}

-(BOOL) isMHRewardAdLoaded{
    NSLog(@"call isRewardLoaded %@",[self.rewardedVideoAd isAdValid]?@"YES":@"NO");
    NSLog(@"call isLoadRewardSuccess %@",self.isLoadRewardSuccess?@"YES":@"NO");
    BOOL adValid = self.isLoadRewardSuccess && [self.rewardedVideoAd isAdValid];
    NSLog(@"call adValid %@",adValid?@"YES":@"NO");
    return adValid;
}

@end

void initHwAds( char *str ,CallbackDelegate callDelegate){
    NSLog(@"HwAdsInterface complete  111111 %s",str);
    callback = callDelegate;
    [[HwAdsInterface sharedInstance] initMHSDK];
}
void showHwRewardAd(char *tag,CallbackDelegate callDelegate){
    NSLog(@"HwAdsInterface complete  showHwRewardAd ");
    callback = callDelegate;
    NSString *str = [NSString stringWithUTF8String:tag];
    [[HwAdsInterface sharedInstance] showMHRewardAd:str];
}
void showHwInterAd(char *tag,CallbackDelegate callback){
    NSLog(@"HwAdsInterface complete  showHwInterAd ");
}
BOOL isHwRewardLoaded(){
    return [[HwAdsInterface sharedInstance] isMHRewardAdLoaded];
}
BOOL isHwInterLoaded(){
    return NO;
}

void TAEventHwPropertie(char *custom, char *str){
    NSLog(@"u3d_parseJson custom: %c", custom);
    NSString *jsonStr = [NSString stringWithCString:str encoding:NSUTF8StringEncoding];
    NSLog(@"u3d_parseJson jsonString: %@", jsonStr);

    NSData *jsonData = [jsonStr dataUsingEncoding:NSUTF8StringEncoding];
    
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:nil];
    NSLog(@"u3d_parseJson dict: %@",dict);
//    NSLog(@"dict - name: %@",dict[@"lev"]);
//    NSLog(@"dict - age: %@",dict[@"login_day"]);
    NSMutableDictionary *dic = [[NSMutableDictionary alloc] initWithDictionary:dict];
    NSLog(@"NSMutableDictionary dict: %@",dic);
//    NSLog(@"NSMutableDictionary - name: %@",dic[@"lev"]);
//    NSLog(@"NSMutableDictionary - age: %@",dic[@"login_day"]);
    if( custom == '\0'){
        custom = "tankclash";
         NSLog(@"u3d_parseJson custom: %c", custom);
    }
    NSString *customKey = [NSString stringWithCString:custom encoding:NSUTF8StringEncoding];
    if(customKey != nil && jsonStr != nil){
        NSLog(@"u3d_parseJson BDAutoTrack: ");
        [LightGameManager lg_event:customKey params:dict];
        
    }
    
    NSArray *keysArray = [dict allKeys];

    for (int i = 0; i < keysArray.count; i++) {
        NSString *key = keysArray[i];
        NSString *value = dict[key];
        if(customKey != nil && key != nil && value != nil){
           NSString *ga = [customKey stringByAppendingFormat:@":%@:%@", key, value ];
            if(ga != nil){
                //[GameAnalytics addDesignEventWithEventId:ga];
            }
            NSLog(@"GameAanlytics Event====%@", ga);
        }
    }
}






