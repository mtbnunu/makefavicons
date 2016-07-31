# MakeFavicons

Ready to be included in build process.

MakeFavicons helps create different sizes of png image on the fly, for direct use on your website.

-----
### [Download Pre-Compiled](https://github.com/mtbnunu/makefavicons/tree/master/MakeFaviconConsole/bin/Release)
-----
### [Setting Up Automated Build](https://github.com/mtbnunu/makefavicons/blob/master/Setup%20Guide/readme.md)
-----

<a name="featues" ></a>
## Features
Given one large png file and some custom settings file, it can create:

  - any number of any size png files
  - apple-touch-icons
  - multi layered .ico file
  - browserconfig.xml for IE
  - manifest.json for Chrome
  - Header Partialview (.cshtml, .php, etc) for section of `<head>` that deals with these images
  - Any other custom config files you might need, dealing with these images.
  - Complete folder structure for all of above

-----


<a name="launch" ></a>
## Launch
you can launch it with or without arguments.

If ran without arguments, It will prompt you for Input PNG file and Output Folder.

<a name="arguments" ></a>
### Arguments
argument | description
---|---
-s | filename of source .png file
-o | directory of output folder. <br>Probably your project root folder.
-w | directory of settings.json and template files. <br>*Without this paremeter, MakeFavicons will expect these files at same location as the .exe file.*
-q | Include this parameter to suppress most console ouputs. <br>Error and Warning will still be shown regardless.
-c | include this parameter to make console exit witout waiting for user imput.<br> *Without this paremeter, "Press any key to exit" message will be shown without this parameter.*
-t | include this parameter to make console ask for file/directory as string from console. <br>*Without this paremeter, console will Pop a window to select file/directory.*

<a name="exitcodes" ></a>
### Exit Codes
code | description
-------- | ---
0 | successfully completed all task
-1 | One or more error or warning was thrown. <br> Might have still created some results, but not all of them.


-----


<a name="preparation" ></a>
## Preparation
Following are needed at minimum to run.
- MakeFavicon.exe
- settings.json

> If you wish to create any config files such as browserconfig.xml or
> manifest.json, **you also need [template] files in the same folder**,
> like following:
> 
> - [browserconfig.xml.template](#browserconfigxmltemplate)
> - [manifest.json.template](#manifestjsontemplate)

<a name="settingsjson" ></a>
### settings.json
Example of a settings.json file
```
{
  "MinInputFileSize": 1200,

  "MakeIco": true,
  "IcoPathAndFileName": "/favicon.ico",

  "SiteRelativeImageFolder": "/Content/img/logos",
  "DefaultOutputFilename": "logo-{0}x{0}.png",

  "AppleTouchIconLocation": "/",
  "AppleTouchIconFilenames": [
    {
      "Id": "apple",
      "Filename": "apple-touch-icon-{0}x{0}.png"
    },
    {
      "Id": "appleprecomposed",
      "Filename": "apple-touch-icon-{0}x{0}-precomposed.png"
    }
  ],

  "OutputFileSizes": [ 16, 32, 48, 64, 128, 250, 256, 500, 512, 1024, 1200, 70, 150, 310, 144, 192 ],
  "AppleTouchIconSizes": [ 57, 72, 76, 114, 120, 152 ],


  "WindowsTileColor": "#ffffff",

  "SiteName": "localhost",
  "SiteUrl": "http://localhost",

  "MakeConfigFiles": [
    {
      "Folder": "/",
      "Filename": "browserconfig.xml"
    },
    {
      "Folder": "/",
      "Filename": "manifest.json"
    },
    {
      "Folder": "/Views/Shared",
      "Filename": "_HEAD-ImagesSection.cshtml"
    }
  ]
}
```


Property  | Type | Required | Description | Example
------------- | ------------- | ------ | --- | ---
<a name="setting_MinInputFileSize"></a>MinInputFileSize  | int | Optional | Sets a minimum dimension for input .png file, to ensure no quality loss. <br>If not provided, there won't be a minimum and image scale-up might occur. | 1200
<a name="setting_MakeIco"></a>MakeIco  | boolean | **Required** | Whether or not to make a .ico file. <br>**To use this feature, [OutputFileSizes] must contain 256.** <br> Output ico file will have layers of 16x16 8bit,<br> 16x16 32bit,<br> 32x32 8bit,<br> 32x32 32bit,<br> 48x48 8bit,<br> 48x48 32bit,<br> 256x256 32bit.<br> Unfortunately this is not customizable at the moment. | true
<a name="setting_IcoPathAndFileName"></a>IcoPathAndFileName | string | *Required if [MakeIco]* | Path and filename of output .ico file. Path is relative to Output Directory. | /favicon.ico
<a name="setting_SiteRelativeImageFolder"></a>SiteRelativeImageFolder | string | **Required** | Path relative to your site, to folder you wish to have the images saved. | /Content/img/logos
<a name="setting_DefaultOutPutFileName"></a>DefaultOutPutFileName | string | *Required if [OutputFileSizes] is not empty*  | Filename structure for default PNG files. {0} will be replaced by dimension. | logo-{0}x{0}.png
<a name="setting_MakeIco"></a>AppleTouchIconLocation | string | *Required if [AppleTouchIconSizes] is not emty* | Path relative to Output Directory to save apple-touch-icon files. You might want it as / (root) to serve the files automatically to crawler. | /
<a name="setting_AppleTouchIconFileNames"></a>AppleTouchIconFileNames | Array of <br>{<br>&nbsp;&nbsp;&nbsp;&nbsp;Id *(string)* ,<br>&nbsp;&nbsp;&nbsp;&nbsp;Filename *(string)* } | *Required if [AppleTouchIconSizes] is not emty* | You probably want to leave this as default (as on the column right to this cell) Id is used in [template] files. | [<br>&nbsp;&nbsp;&nbsp;&nbsp;{<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"Id": "apple",<br>&nbsp;&nbsp;&nbsp;&nbsp;"Filename":"apple-touch-icon-{0}x{0}.png"<br>&nbsp;&nbsp;&nbsp;&nbsp;},<br>&nbsp;&nbsp;&nbsp;&nbsp;{ <br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"Id": "appleprecomposed",<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"Filename":"apple-touch-icon-{0}x{0}-precomposed.png"<br>&nbsp;&nbsp;&nbsp;&nbsp;}<br>]
<a name="setting_OutputFileSizes"></a>OutputFileSizes | Array of int | **Required**<br>- *can be **empty array** to not make any.* | Dimensions of Output files. <br>These will be made and saved to [WorkingDirectiory]/[SiteRelativeImageFolder] using [DefaultOutputFilename].  | [ 16, 32, 48, 64, 128, 250, 256, 500, 512, 1024, 1200, 70, 150, 310, 144, 192 ]
<a name="setting_AppleTouchIconSizes"></a>AppleTouchIconSizes | Array of int | **Required** <br> - *can be **empty array** to not make any.* | Dimensions of apple-touch-icons you wish to make. These will be saved to [WorkingDirectory]/[AppleTouchIconLocation] using [AppleTouchIconFileNames]. <br>*(multiple files of same dimension will be created with each filename)* | [ 57, 72, 76, 114, 120, 152 ]
<a name="setting_MakeConfigFiles"></a>MakeConfigFiles | Array of <br>{<br>&nbsp;&nbsp;&nbsp;&nbsp;Folder *(string)* , <br>&nbsp;&nbsp;&nbsp;&nbsp;Filename *(string)* <br>} | **Required** <br>- can be **empty array** to not make any config files | Folder and Filenames of any [config-files]. For each of these entries, you MUST have corresponding [template] file in the same folder as settings.json | [<br>&nbsp;&nbsp;&nbsp;&nbsp;{<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"Folder": "/",<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"Filename":"browserconfig.xml"<br>&nbsp;&nbsp;&nbsp;&nbsp;},<br>&nbsp;&nbsp;&nbsp;&nbsp;{<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"Folder": "/",<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"Filename": "manifest.json"<br>},<br>&nbsp;&nbsp;&nbsp;&nbsp;{<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"Folder": "/",<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"Filename": "_HEAD-ImagesSection.cshtml"<br>&nbsp;&nbsp;&nbsp;&nbsp;}<br>]

> Any Additional Setting can be added here to be used in [template]
> files, for example

Property | Value
-----|------------
WindowsTileColor | "#ffffff"
SiteName | "localhost"
SiteUrl | "http://localhost"

You will see how these come useful in [template] files soon.

<a name="tamplate"></a>
### Templates

For each [MakeConfigFiles], you must have a corresponding template file with filename &lt;[MakeConfigFile](#setting_makeconfigfiles).FileName&gt;.template.

For example, having this setting:

```sh
  "MakeConfigFiles": [
    {
      "Folder": "/",
      "Filename": "browserconfig.xml"
    },
    {
      "Folder": "/",
      "Filename": "manifest.json"
    },
    {
      "Folder": "/",
      "Filename": "_HEAD-ImagesSection.cshtml"
    }
  ]
```
you must have following template files in the same directory as settings.json.

- browserconfig.xml.template
- manifest.json.template
- _HEAD-ImagesSection.cshtml.template

Each template might look something like following.

<a name="browserconfigxmltemplate"></a>
#### browserconfig.xml.template
```sh
<?xml version="1.0" encoding="utf-8"?>
<browserconfig>
  <msapplication>
    <tile>
      <square70x70logo src="{{path:70}}"/>
      <square150x150logo src="{{path:150}}"/>
      <square310x310logo src="{{path:310}}"/>
      <TileColor>{{setting:WindowsTileColor}}</TileColor>
    </tile>
  </msapplication>
</browserconfig>
```

<a name="manifestjsontemplate"></a>
#### manifest.json.template
```sh
{
    "name":"{{setting:SiteName}}",
    "icons":[
        {"src":"{{path:48}}","sizes":"48x48","type":"image/png"},
        {"src":"{{path:144}}","sizes":"144x144","type":"image/png"},
        {"src":"{{path:192}}","sizes":"192x192","type":"image/png"}
    ],
    "start_url":"/",
    "display":"standalone",
    "orientation":"portrait"
}
```

<a name="headtemplate"></a>

#### _Head-ImagesSection.cshtml.template
```sh
[[if true setting:MakeIco]]
<link rel=icon type="image/x-icon" sizes="16x16 32x32 48x48 256x256" href="{{path:ico}}">
[[endif]]
<link rel=apple-touch-icon-precomposed href="{{path:apple-152}}">
[[if exists path:manifest.json]]
<link rel=manifest href="{{path:manifest.json}}">
[[endif]]
[[if exists path:browserconfig.xml]]
<meta name=msapplication-config content="{{path:browserconfig.xml}}">
[[endif]]
<meta property="og:image" content="{{setting:SiteUrl}}{{path:1200}}">
```

There are a few snippets you can use in the template files.

Broadly, there are two types that look something like following:

- **Substitutions** 
  - Format: `{{<prefix>:<val>}}`
  - Substitue the area with corresponding value
- **Conditionals** 
	- Format: `[[if ....]] ...YOUR CODE HERE... [[endif]]`
	- Only write the enclosed area if condition is met.

<a name="substitutions"></a>
#### Substitutions
Supported values
prefix | value | example snippet | example output
---- | ---- | ---- | ----
setting | "any setting from [settings.json]<br><br>*Make sure the type can be converted to string safely. For example, array or object is not safe.* | {{setting:SiteName}} | YourSiteName
path | one of items in [OutputFileSizes array] | {{path:32}} | /Content/img/logos/mylogo-32x32.png
path | one of items in [AppleTouchIconFilenames].Id followed by a dash and one of items in [AppleTouchIconSizes] | {{path:apple-152}} | /apple-touch-icon-152x152.png
 | | | {{path:appleprecomposed-120}} | /apple-touch-icon-120x120-precomposed.png
 path | one of items in [MakeConfigFiles]->Filename | {{path:manifest.json}} | /manifest.json
 path | ico | {{path:ico}} | /favicon.ico
 

 <a name="conditionals"></a>
#### Conditionals
 There are only two types of conditionals :
 
 - **if true**
	 - Checks if given value is boolean true
	 - Currently only supports prefix ***setting***
	 - Format: `[[if true setting:<val>]]...[[endif]]`
 - **if exists**
	 - Checks if given value exists (not null)
	 - Format: `[[if exists <prefix>:<val>]]...[[endif]]`
  
 If Type | prefix | val | example snippet | Comment
----|----|----|----| ----
true | setting | any setting value | [[if true setting:MakeIco]] some code [[endif]] | If the setting does not exist or is not boolean, MakeFavicon will throw an error.
exists | setting | any setting value | [[if exists setting:MySetting]] some code [[endif]] | This conditional will check for "existence" of the setting, not the boolean value of the setting. If the setting is not defined or null, it will yield false, otherwie true.
exists | path | any of paths mentioned in [substitutions] section | [[if exists path:manifest.json]] some code [[endif]] | This conditional will check if such path has been created in the corresponding build.

> You CAN use Substitutions inside Conditionals, 
> But you **CANNOT use nested conditionals.**

<a name="example"></a>
# Example
<a name="exampleinput"></a>
## Input
### In MakeFavicon Folder :
 - MakeFavicons.exe
 - [settings.json]
 - [browserconfig.xml.template]
 - [manifest.json.template]
 - [_Head-ImagesSection.cshtml.template]
  
each with content exactly same as found above,
will yield output:
<a name="exampleoutput"></a>
## Output
### In Work Directory : 
 - Content
    - img
       -  logos
          -  logo-16x16.png
          -  logo-32x32.png
          -  logo-48x48.png
          -  logo-64x64.png
          -  logo-70x70.png
          -  logo-128x128.png
          -  logo-144x144.png
          -  logo-150x150.png
          -  logo-192x192.png
          -  logo-250x250.png
          -  logo-256x256.png
          -  logo-310x310.png
          -  logo-500x500.png
          -  logo-512x512.png
          -  logo-1024x1024.png
          -  logo-1200x1200.png
    - Views
        - Shared
            - [_Head-ImagesSection.cshtml](#outputHEAD)
-  favicon.ico
-  apple-touch-icon-57x57.png
-  apple-touch-icon-57x57-precomposed.png
-  apple-touch-icon-72x72.png
-  apple-touch-icon-72x72-precomposed.png
-  apple-touch-icon-76x76.png
-  apple-touch-icon-76x76-precomposed.png
-  apple-touch-icon-114x114.png
-  apple-touch-icon-114x114-precomposed.png
-  apple-touch-icon-120x120.png
-  apple-touch-icon-120x120-precomposed.png
-  apple-touch-icon-152x152.png
-  apple-touch-icon-152x152-precomposed.png
-  [browserconfig.xml](#outputBrowserConfig)
-  [manifest.json](#ouputManifest)

<a name="ouputHEAD"></a>
### in output _Head-ImagesSection.cshtml
```sh
<link rel=icon type="image/x-icon" sizes="16x16 32x32 48x48 256x256" href="/favicon.ico">
<link rel=apple-touch-icon-precomposed href="/apple-touch-icon-152x152.png">
<link rel=manifest href="/manifest.json">
<meta name=msapplication-config content="/browserconfig.xml">
<meta property="og:image" content="http://localhost/Content/img/logos/logo-1200x1200.png">
```
<a name="outputBrowserConfig"></a>
### in output browserconfig.xml
```
<?xml version="1.0" encoding="utf-8"?>
<browserconfig>
  <msapplication>
    <tile>
      <square70x70logo src="/Content/img/logos/logo-70x70.png"/>
      <square150x150logo src="/Content/img/logos/logo-150x150.png"/>
      <square310x310logo src="/Content/img/logos/logo-310x310.png"/>
      <TileColor>#ffffff</TileColor>
    </tile>
  </msapplication>
</browserconfig>
```

<a name="ouputManifest"></a>
### in output manifest.json
```sh
{
    "name":"localhost",
    "icons":[
        {"src":"/Content/img/logos/logo-48x48.png","sizes":"48x48","type":"image/png"},
        {"src":"/Content/img/logos/logo-144x144.png","sizes":"144x144","type":"image/png"},
        {"src":"/Content/img/logos/logo-192x192.png","sizes":"192x192","type":"image/png"}
    ],
    "start_url":"/",
    "display":"standalone",
    "orientation":"portrait"
}
```

[settings.json]:#settingsjson
[WorkingDirectory]:#workingdirectory
[template]: #template
[browserconfig.xml.template]:#browserconfigxmltemplate
[manifest.json.template]:#manifestjsontemplate
[_Head-ImagesSection.cshtml.template]:#headtemplate
[config-files]:#template
[MakeConfigFiles]:#setting_MakeConfigFiles
[AppleTouchIconFilenames]:#setting_AppleTouchIconFilenames
[AppleTouchIconSizes]:#setting_AppleTouchIconSizes
[AppleTouchIconLocation]:#setting_AppleTouchIconLocation
[SiteRelativeImageFolder]:#setting_SiteRelativeImageFolder
[DefaultOutputFilename]:#setting_DefaultOutputFilename
[OutputFileSizes]:#setting_OutputFileSizes
[MakeIco]:#setting_MakeIco
