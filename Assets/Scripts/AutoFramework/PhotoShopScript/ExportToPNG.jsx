// This script exports photoshop layers as individual PNGs. It also
// writes a JSON file that can be imported into Spine where the images
// will be displayed in the same positions and draw order.

// Setting defaults.
var writePngs = true;
var writeTemplate = false;
var writeJson = true;
var ignoreHiddenLayers = true;
var pngScale = 1;
var groupsAsSkins = false;
var trimWhitespace = false;
var saveDir = "C:/Images/";


// IDs for saving settings.
const settingsID = stringIDToTypeID("settings");
const writePngsID = stringIDToTypeID("writePngs");
const writeTemplateID = stringIDToTypeID("writeTemplate");
const writeJsonID = stringIDToTypeID("writeJson");
const ignoreHiddenLayersID = stringIDToTypeID("ignoreHiddenLayers");
const groupsAsSkinsID = stringIDToTypeID("groupsAsSkins");
const trimWhitespaceID = stringIDToTypeID("trimWhitespace");
const pngScaleID = stringIDToTypeID("pngScale");
const saveDirID = stringIDToTypeID("saveDir");
//const jsonDirID = stringIDToTypeID("jsonDir");
//const paddingID = stringIDToTypeID("padding");

var originalDoc;
try {
	originalDoc = app.activeDocument;
} catch (ignored) {}
var settings, progress;
loadSettings();
showDialog();


function run () {
	saveSettings();
	showProgress();

	// create path
	if(!endsWith(saveDir,"/"))
		 saveDir+="/";
	 new Folder(saveDir).create();;

	// Get ruler origin.
	//var xOffSet = 0, yOffSet = 0;
	// if (useRulerOrigin) {
	// 	var ref = new ActionReference();
	// 	ref.putEnumerated(charIDToTypeID("Dcmn"), charIDToTypeID("Ordn"), charIDToTypeID("Trgt"));
	// 	var desc = executeActionGet(ref);
	// 	xOffSet = desc.getInteger(stringIDToTypeID("rulerOriginH")) >> 16;
	// 	yOffSet = desc.getInteger(stringIDToTypeID("rulerOriginV")) >> 16;
	// }

	activeDocument.duplicate();
	// Output template image.
	if (writeTemplate) {
		if (pngScale != 1) {
			scaleImage();
			storeHistory();
		}

		var file = new File(saveDir + "template" + ".png");
		if (file.exists) file.remove();

		savePNG(file);

		if (pngScale != 1) restoreHistory();
	}

	if (!writeJson && !writePngs) {
		activeDocument.close(SaveOptions.DONOTSAVECHANGES);
		return;
	}
	//function getLayers(layers){
	//	for(var i =0;i<layers.length;i++){
	//		var myLayer = layers[i];
	//		if(myLayer.typename == "LayerSet")
	//		{
	//			alert("111 myLayer name:"+myLayer.name);
	//			getLayers(myLayer.layers);
	//		}
	//		else{
	//			alert("111 myLayer name:"+myLayer.name +",kind:"+myLayer.kind );
	//			if(myLayer && myLayer.kind == LayerKind.TEXT){
	//				var text = myLayer.textItem;
	//				var contents = text.contents;
	//				var color = text.color.rgb["hexValue"];
	//				var opacity = myLayer.opacity;
	//				var size = text.size.value;
	//				alert("text contents:"+contents+",color:"+color+",opacity:"+opacity + ",size:"+size );
	//			}
	//		}
			
	//	}
	//}
	//getLayers(activeDocument.layers);
	//return;
	
	// Rasterize all layers.
	try {
		//executeAction(stringIDToTypeID( "rasterizeAll" ), undefined, DialogModes.NO);
	} catch (ignored) {}

	// Collect and hide layers.
	
	
	var layers = [];
	collectLayers(activeDocument, layers);
	var layersCount = layers.length;

	storeHistory();
	//var json='{"sort":[\n';
	var json='{\n"data":{';
	// json+=saveDir+"\n";
	// json+=imagesFolder+"\n";
	json+='"width":'+activeDocument.width.as("px")+",";
	json+='"height":'+activeDocument.height.as("px");
	json+="},\n";
	json+='"sort":[\n';
	// Store the slot names and layers for each skin.
	
	var slots = {}, skins = { "root": [] };
	for (var i = layersCount - 1; i >= 0; i--) {
		var layer = layers[i];

		json += '\t{"name":"' + layer.name+ '","id":"' + layer.id + '","index":"' + i + '"}';
		if(i>0)
		{
			json += ",\n";
		}
		// Use groups as skin names.
		var potentialSkinName = trim(layer.parent.name);
		var layerGroupSkin = potentialSkinName.indexOf("-NOSKIN") == -1;
		var skinName = (groupsAsSkins && layer.parent.typename == "LayerSet" && layerGroupSkin) ? potentialSkinName : "root";

		var skinLayers = skins[skinName];
		if (!skinLayers) skins[skinName] = skinLayers = [];
		skinLayers[skinLayers.length] = layer;

		slots[layerName(layer)] = true;
	}

	// Output skeleton and bones.
	//var json = '{"skeleton":{"images":"' + relsaveDir + '"},\n"bones":[{"name":"root"}],\n"slots":[\n';
	 //var json = '{"PNGs":{\n';
	// var json='{"sort":[\n';
	// // Output slots.

	// var slotsCount = countAssocArray(slots);
	// var slotIndex = 0;
	// for (var slotName in slots) {
	// 	//if (!slots.hasOwnProperty(slotName)) continue;
	// 	// Use image prefix if slot's attachment is in the default skin.		
	// 	var attachmentName = slotName;
	// 	// var skinn=skinName;
	// 	// var defaultSkinLayers = skins[skinName];
	// 	// 	for (var i = defaultSkinLayers.length - 1; i >= 0; i--) {
	// 	// 		if (layerName(defaultSkinLayers[i]) == slotName) {
	// 	// 			attachmentName = slotName;
	// 	// 			break;
	// 	// 		}
	// 	// 	}
	// 	//json += '\t{"name":"' + slotName + '","bone":"' + skinn + '","attachment":"' + attachmentName + '"}';
	// 	json += '\t"' + slotName  + '"';
	// 	slotIndex++;
	// 	json += slotIndex < slotsCount ? ",\n" : "\n";
	// }
	 json += '\n],\n"pngdata":{\n';

	// Output skins.
	var skinsCount = countAssocArray(skins);
	var skinIndex = 0;
	for (var skinName in skins) {
		if (!skins.hasOwnProperty(skinName)) continue;
		json += '\t"' + skinName + '":[\n';

		var skinLayers = skins[skinName];
		var skinLayersCount = skinLayers.length;
		var skinLayerIndex = 0;
		for (var i = skinLayersCount - 1; i >= 0; i--) {
			var layer = skinLayers[i];
			
			var slotName = layerName(layer);
			var placeholderName, attachmentName;
			if (skinName == "root") {
				placeholderName = slotName;
				attachmentName = placeholderName;
			} else {
				placeholderName = slotName;
				attachmentName = skinName + "/" + slotName;
			}

			var x = activeDocument.width.as("px") * pngScale;
			var y = activeDocument.height.as("px") * pngScale;
			layer.visible = true;
			if(trimWhitespace)
			{
				if (!layer.isBackgroundLayer) activeDocument.trim(TrimType.TRANSPARENT, false, true, true, false);
				x -= activeDocument.width.as("px") * pngScale;
				y -= activeDocument.height.as("px") * pngScale;
				if (!layer.isBackgroundLayer) activeDocument.trim(TrimType.TRANSPARENT, true, false, false, true);
			}
			//var width = activeDocument.width.as("px") * pngScale + padding * 2;
			//var height = activeDocument.height.as("px") * pngScale + padding * 2;
			var width = activeDocument.width.as("px") * pngScale ;
			var height = activeDocument.height.as("px") * pngScale;
				
			// Save image.
			if (writePngs) {
				if (pngScale != 1) scaleImage();
				//if (padding > 0) activeDocument.resizeCanvas(width, height, AnchorPosition.MIDDLECENTER);

				if (skinName != "root") new Folder(saveDir + skinName).create();
				if(attachmentName.indexOf(" ",)>0)
				{
					alert("The layer name contains Spaces, Please rename it!!!\nͼ�����ְ����ո�,��������!!!"+attachmentName); //you kong ge 
					return;
				}
				
				if(layer.kind != LayerKind.TEXT)	
					savePNG(new File(saveDir + attachmentName + ".png"));
			}
			restoreHistory();
			layer.visible = false;

			x += Math.round(width) / 2;
			y += Math.round(height) / 2;

			// Make relative to the Photoshop document ruler origin.
			// if (useRulerOrigin) {
			// 	x -= xOffSet * pngScale;
			// 	y -= activeDocument.height.as("px") * pngScale - yOffSet * pngScale; // Invert y.
			// }

			//json += '\t"' + slotName + '":{'+ '"x":' + x + ',"y":' + y + ',"width":' + Math.round(width) + ',"height":' + Math.round(height) + '}';
			
			
			var pngType = "image";
			var contents = "";
			var rgb = "";
			var opacity = 1;
			var size = 0;
			if(layer && layer.kind == LayerKind.TEXT)
			{
				var text = layer.textItem;
				pngType = "text";
				contents = text.contents;
				//if(contents.indexOf("\n"))
				//{
					//contents.replace("\n","");
					//alert("contents:"+contents); 
					//return;
				//}
				contents = contents.replace(/\r|\n/ig,"");//ȥ������
				rgb = text.color.rgb["hexValue"];
				opacity = layer.opacity / 100; 
				size = text.size.value;
				//if(text.justification == Justification.LEFT)
				//	justification = "left";
				//else if(text.justification == Justification.CENTER)
				//	justification = "center";
				//else if(text.justification == Justification.RIGHT)
				//	justification = "right";
			}
			//����ͼƬ���ı����json
			json += '\t{"pngname":"' + slotName +'","pngType":"' + pngType+ '","contents":"'+contents+ '","id":' + layer.id + ',"x":' + x + ',"y":' + y + ',"width":' + Math.round(width) + ',"height":' + Math.round(height) +',"rgb":"'+rgb+'","opacity":'+opacity+',"size":'+size+'}';
			
			//ȫ����ͼƬ����
			//json += '\t{"pngname":"' + slotName + '","id":' + layer.id + ',"x":' + x + ',"y":' + y + ',"width":' + Math.round(width) + ',"height":' + Math.round(height) + '}';
			
			
			//json += '\t\t"' + layer.id + '":{'+ '"x":' + x + ',"y":' + y + ',"width":' + Math.round(width) + ',"height":' + Math.round(height) + '}';
			// if (attachmentName == placeholderName) {
			// } else {
			// 	json += '\t\t"' + slotName + '":{'
			// 		+ '"x":' + x + ',"y":' + y + ',"width":' + Math.round(width) + ',"height":' + Math.round(height) + '}';
			// }

			skinLayerIndex++;
			json += skinLayerIndex < skinLayersCount ? ",\n" : "\n";
		}
		json += "\t\]";

		skinIndex++;
		json += skinIndex < skinsCount ? ",\n" : "\n";
	}
	json += '}}';

	activeDocument.close(SaveOptions.DONOTSAVECHANGES);

	// Output JSON file.
	if (writeJson) {
		var name = decodeURI(originalDoc.name);
		name = name.substring(0, name.indexOf("."));
		var file = new File(saveDir + name + ".ps.json");
		file.remove();
		file.open("w", "TEXT");
		file.lineFeed = "\n";
		file.encoding = "UTF-8";
		file.write(json);
		file.close();
	}
	
	alert("Success Export !!!");

}
function showProgress (title, total) {
	title = "Export Status"
	if (!progress) {
		var dialog = new Window("palette", title);
		dialog.alignChildren = "fill";
		dialog.orientation = "column";

		var message = dialog.add("statictext", undefined, "Exporting...");

		var group = dialog.add("group");
			var bar = group.add("progressbar");
			bar.preferredSize = [300, 16];
			bar.maxvalue = total;
			bar.value = 1;
			var cancelButton = group.add("button", undefined, "Cancel");

		cancelButton.onClick = function () {
			cancel = true;
			cancelButton.enabled = false;
			return;
		};

		dialog.center();
		dialog.show();
		dialog.active = true;

		progress = {
			dialog: dialog,
			bar: bar,
			message: message
		};
	} else {
		progress.dialog.text = title;
		progress.bar.maxvalue = total;
	}
	progress.count = 0;
	progress.total = total;
	progress.updateTime = 0;
	var reset = $.hiresTimer;
}

function incrProgress (text) {
	progress.count++;
	if (progress.count != 1 && progress.count < progress.total) {
		progress.updateTime += $.hiresTimer;
		if (progress.updateTime < 500000) return;
		progress.updateTime = 0;
	}
	text = progress.count + " / "+ progress.total + ": " + trim(text);
	progress.bar.value = progress.count;
	progress.message.text = text;
	if (!progress.dialog.active) progress.dialog.active = true;
}

function showDialog () {
	if (!originalDoc) {
		alert("Please open a document before running the LayersToPNG script.");
		return;
	}
	if (!hasFilePath()) {
		alert("Please save the document before running the LayersToPNG script.");
		return;
	}

	var dialog = new Window("dialog", "LayersToPNG1.1");
	dialog.alignChildren = "fill";

	var checkboxGroup = dialog.add("group");
		var group = checkboxGroup.add("group");
			group.orientation = "column";
			group.alignChildren = "left";
			var writePngsCheckbox = group.add("checkbox", undefined, " Write Layer PNGs");
			writePngsCheckbox.value = writePngs;
			var writeTemplateCheckbox = group.add("checkbox", undefined, " Write a template PNG");
			writeTemplateCheckbox.value = writeTemplate;
			var writeJsonCheckbox = group.add("checkbox", undefined, " Write JSON");
			writeJsonCheckbox.value = writeJson;
		group = checkboxGroup.add("group")
			group.orientation = "column";
			group.alignChildren = "left";
			var ignoreHiddenLayersCheckbox = group.add("checkbox", undefined, " Ignore hidden layers");
			ignoreHiddenLayersCheckbox.value = ignoreHiddenLayers;
			var groupsAsSkinsCheckbox = group.add("checkbox", undefined, " Use groups");
			groupsAsSkinsCheckbox.value = groupsAsSkins;
			 var groupsTrimWhiteSpaceCheckbox = group.add("checkbox", undefined, " Trim Whitespace");
			groupsTrimWhiteSpaceCheckbox.value = trimWhitespace;
			checkboxGroup.alignment = ["fill", ""];
			checkboxGroup.alignChildren = ["fill", ""];

	var slidersGroup = dialog.add("group");
		group = slidersGroup.add("group");
			group.orientation = "column";
			group.alignChildren = "right";
			group.add("statictext", undefined, "PNG scale:");
			//group.add("statictext", undefined, "Padding:");
		group = slidersGroup.add("group");
			group.orientation = "column";
			var scaleText = group.add("edittext", undefined, pngScale * 100);
			scaleText.characters = 4;
			//var paddingText = group.add("edittext", undefined, padding);
			//paddingText.characters = 4;
		group = slidersGroup.add("group");
			group.orientation = "column";
			group.add("statictext", undefined, "%");
			//group.add("statictext", undefined, "px");
		group = slidersGroup.add("group");
			group.alignment = ["fill", ""];
			group.orientation = "column";
			group.alignChildren = ["fill", ""];
			var scaleSlider = group.add("slider", undefined, pngScale * 100, 1, 100);
			//var paddingSlider = group.add("slider", undefined, padding, 0, 4);
	scaleText.onChanging = function () { scaleSlider.value = scaleText.text; };
	scaleSlider.onChanging = function () { scaleText.text = Math.round(scaleSlider.value); };
	//paddingText.onChanging = function () { paddingSlider.value = paddingText.text; };
	//paddingSlider.onChanging = function () { paddingText.text = Math.round(paddingSlider.value); };

	var outputGroup = dialog.add("panel", undefined, "Output Directory");
		outputGroup.alignChildren = "fill";
		outputGroup.margins = [10,15,10,10];
		outputGroup.add("statictext", undefined, "absolute path (����·��) eg:c:/aaa/bb/cc").alignment = "center";
		var textGroup = outputGroup.add("group");
			group = textGroup.add("group");
				group.orientation = "column";
				group.alignChildren = "right";
				group.add("statictext", undefined, "Save Path:");
				//group.add("statictext", undefined, "JSON:");
			group = textGroup.add("group");
				group.orientation = "column";
				group.alignChildren = "fill";
				group.alignment = ["fill", ""];
				var saveDirText = group.add("edittext", undefined, saveDir)
				saveDirText.preferredSize=[180,20];
				//var jsonDirText = group.add("edittext", undefined, jsonDir);
		var btnBrowse =textGroup.add("button",undefined,"Browse");
		btnBrowse.preferredSize=[40,15];
		btnBrowse.onClick=function(){
		 	var outputFolder=Folder.selectDialog("ѡ�񱣴�Ŀ¼");
		 	if(outputFolder!=null)
		 	{
				saveDirText.text=outputFolder;
				//alert(outputFolder);
		 	}
		};

		
		//outputGroup.add("statictext", undefined, "Begin paths with \"./\" to be relative to the PSD file.").alignment = "center";

	var group = dialog.add("group");
		group.alignment = "center";
		var runButton = group.add("button", undefined, "OK");
		var cancelButton = group.add("button", undefined, "Cancel");
		cancelButton.onClick = function () {
			dialog.close(0);
			return;
		};

	function updateSettings () {
		writePngs = writePngsCheckbox.value;
		writeTemplate = writeTemplateCheckbox.value;
		writeJson = writeJsonCheckbox.value;
		ignoreHiddenLayers = ignoreHiddenLayersCheckbox.value;
		var scaleValue = parseFloat(scaleText.text);
		if (scaleValue > 0 && scaleValue <= 100) pngScale = scaleValue / 100;
		groupsAsSkins = groupsAsSkinsCheckbox.value;
		trimWhitespace=groupsTrimWhiteSpaceCheckbox.value;
		saveDir = saveDirText.text;
		
		// var paddingValue = parseInt(paddingText.text);
		// if (paddingValue >= 0) padding = paddingValue;
	}

	dialog.onClose = function() {
		updateSettings();
		saveSettings();
	};
	
	runButton.onClick = function () {
		if (scaleText.text <= 0 || scaleText.text > 100) {
			alert("PNG scale must be between > 0 and <= 100.");
			return;
		}
		// if (paddingText.text < 0) {
		// 	alert("Padding must be >= 0.");
		// 	return;
		// }
		dialog.close(0);

		var rulerUnits = app.preferences.rulerUnits;
		app.preferences.rulerUnits = Units.PIXELS;
		try {
			run();
		} catch (e) {
			alert("An unexpected error has occurred.\n\nTo debug, run the LayersToPNG script using Adobe ExtendScript "
				+ "with \"Debug > Do not break on guarded exceptions\" unchecked.");
			debugger;
		} finally {
			if (activeDocument != originalDoc) activeDocument.close(SaveOptions.DONOTSAVECHANGES);
			app.preferences.rulerUnits = rulerUnits;
		}
	};

	dialog.center();
	dialog.show();
}

function loadSettings () {
	try {
		settings = app.getCustomOptions(settingsID);
	} catch (e) {
		saveSettings();
	}
	if (typeof settings == "undefined") saveSettings();
	settings = app.getCustomOptions(settingsID);
	if (settings.hasKey(writePngsID)) writePngs = settings.getBoolean(writePngsID);
	if (settings.hasKey(writeTemplateID)) writeTemplate = settings.getBoolean(writeTemplateID);
	if (settings.hasKey(writeJsonID)) writeJson = settings.getBoolean(writeJsonID);
	if (settings.hasKey(ignoreHiddenLayersID)) ignoreHiddenLayers = settings.getBoolean(ignoreHiddenLayersID);
	if (settings.hasKey(pngScaleID)) pngScale = settings.getDouble(pngScaleID);
	if (settings.hasKey(groupsAsSkinsID)) groupsAsSkins = settings.getBoolean(groupsAsSkinsID);
	if (settings.hasKey(trimWhitespaceID)) trimWhitespace = settings.getBoolean(trimWhitespaceID);
	if (settings.hasKey(saveDirID)) saveDir = settings.getString(saveDirID);
	//if (settings.hasKey(jsonDirID)) jsonDir = settings.getString(jsonDirID);
}

function saveSettings () {
	var settings = new ActionDescriptor();
	settings.putBoolean(writePngsID, writePngs);
	settings.putBoolean(writeTemplateID, writeTemplate);
	settings.putBoolean(writeJsonID, writeJson);
	settings.putBoolean(ignoreHiddenLayersID, ignoreHiddenLayers);
	settings.putDouble(pngScaleID, pngScale);
	settings.putBoolean(groupsAsSkinsID, groupsAsSkins);
	settings.putBoolean(trimWhitespaceID, trimWhitespace);
	settings.putString(saveDirID, saveDir);
	app.putCustomOptions(settingsID, settings, true);
}

// Photoshop utility:

function scaleImage () {
	var imageSize = activeDocument.width.as("px");
	activeDocument.resizeImage(UnitValue(imageSize * pngScale, "px"), null, 300, ResampleMethod.BICUBICSHARPER);
}

var historyIndex;
function storeHistory () {
	historyIndex = activeDocument.historyStates.length - 1;
}
function restoreHistory () {
	activeDocument.activeHistoryState = activeDocument.historyStates[historyIndex];
}

function collectLayers (layer, collect) {
	for (var i = 0, n = layer.layers.length; i < n; i++) {
		var child = layer.layers[i];
		if (ignoreHiddenLayers && !child.visible) continue;
		if (child.bounds[2] == 0 && child.bounds[3] == 0) continue;
		if (child.layers && child.layers.length > 0)
			collectLayers(child, collect);
		//else if (child.kind == LayerKind.NORMAL) {
		//	collect.push(child);
		//	child.visible = false;
		//}
		else{
			collect.push(child);
			child.visible = false;
		}
	}
}

function hasFilePath () {
	var ref = new ActionReference();
	ref.putEnumerated(charIDToTypeID("Dcmn"), charIDToTypeID("Ordn"), charIDToTypeID("Trgt"));
	return executeActionGet(ref).hasKey(stringIDToTypeID("fileReference"));
}

function absolutePath (path) {
	path = forwardSlashes(trim(path));
	if (!startsWith(path, "./")) {
		var absolute = decodeURI(new File(path).fsName);
		if (!startsWith(absolute, decodeURI(new File("child").parent.fsName))) return forwardSlashes(absolute) + "/";
		path = "./" + path;
	}
	if (path.length == 0)
		path = decodeURI(activeDocument.path);
	else if (startsWith(path, "./"))
		path = decodeURI(activeDocument.path) + path.substring(1);
	path = (new File(path).fsName).toString();
	path = forwardSlashes(path);
	if (path.substring(path.length - 1) != "/") path += "/";
	return path;
}
function forwardSlashes (path) {
	return path.replace(/\\/g, "/");
}
// JavaScript utility:

function countAssocArray (obj) {
	var count = 0;
	for (var key in obj)
		if (obj.hasOwnProperty(key)) count++;
	return count;
}
function countAssocArray2 (obj) {
	var count = 0;
	for (var key in obj)
		 count++;
	return count;
alignment}

function trim (value) {
	return value.replace(/^\s+|\s+$/g, "");
}



function stripSuffix (str, suffix) {
	if (endsWith(str.toLowerCase(), suffix.toLowerCase())) str = str.substring(0, str.length - suffix.length);
	return str;
}

function layerName (layer) {
	return stripSuffix(trim(layer.name), ".png").replace(/[:\/\\*\?\"\<\>\|]/g, "");
}
function indexOf (array, value) {
	for (var i = 0, n = array.length; i < n; i++)
		if (array[i] == value) return i;
	return -1;
}

function trim (value) {
	return value.replace(/^\s+|\s+$/g, "");
}

function startsWith (str, prefix) {
	return str.indexOf(prefix) === 0;
}

function endsWith (str, suffix) {
	return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

function quote (value) {
	return '"' + value.replace(/"/g, '\\"') + '"';
}

function forwardSlashes (path) {
	return path.replace(/\\/g, "/");
}

function savePNG (file) {
	// SaveForWeb changes spaces to dash. Also some users report it writes HTML.
	//var options = new ExportOptionsSaveForWeb();
	//options.format = SaveDocumentType.PNG;
	//options.PNG8 = false;
	//options.transparency = true;
	//options.interlaced = false;
	//options.includeProfile = false;
	//activeDocument.exportDocument(file, ExportType.SAVEFORWEB, options);

	// SaveAs sometimes writes a huge amount of XML in the PNG. Ignore it or use Oxipng to make smaller PNGs.

	//bao chi yuan lai de DPI
	var options = new PNGSaveOptions();
	options.compression = 6;
	activeDocument.saveAs(file, options, true, Extension.LOWERCASE); 
}