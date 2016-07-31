# How to use as part of Visual Studio Build
> The project in this guide is a Asp.Net5 App, with Razor Views.
> Of course, you can use this with any project types, as long as you change the settings.json accordingly.


### In my project I created a folder called _FaviconMaker.
<MAKEFAVIOCNFOLDER PNG>
![](blob:chrome-extension%3A//cdneggiaafcmelgcaiihmhiagieomgfj/d69c5814-b2f6-454e-961c-8db001d900b2)

This folder contains settings, templates, and input png file called faviconsource.png.
The faviconsource.png file is what I work with regarding logo now, not any generated files.

My _HeadImagePartial.cshtml.template:

```
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
<meta name="twitter:image" content="{{setting:SiteUrl}}{{path:1200}}" />
```

and I have specified the output folder of this to /Views/Shared, like below:
```
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
      "Filename": "_HeadImagePartial.cshtml"
    }
  ]
}
```
### Include this to Build Event

**Go to Project - > Properties -> Build Events**

![](blob:chrome-extension%3A//cdneggiaafcmelgcaiihmhiagieomgfj/9d6d1581-4e2b-470a-80c1-a0375883eb27)

**Click on Edit Pre-build->Macros to use variables for solution/project folders.**
![](blob:chrome-extension%3A//cdneggiaafcmelgcaiihmhiagieomgfj/08d2e67e-b011-41e3-b78c-2150910d8eac)

Here's my version.
```
D:\dev\makefavicons\MakeFavicons\MakeFaviconConsole\bin\Release\MakeFaviconConsole.exe -s $(ProjectDir)\_FaviconMaker\faviconsource.png -o $(ProjectDir) -w $(ProjectDir)\_FaviconMaker\ -c
```
>** Make sure to use arguments, especially -s, -o, -w, -c**

## That's It. Build the project.

### Here's what I got:

/Content/img/logos folder was my ***SiteRelativeImageFolder***
![](blob:chrome-extension%3A//cdneggiaafcmelgcaiihmhiagieomgfj/355eca00-0cdd-4b4c-83fe-d976a9ddebc7)

"/" was my ***AppleTouchIconLocation***
This is a bit messy, but ill leave them there for now, since I don't want to write url-rewrite rules.

Also notice the *browserconfig.xml* and manifest*.json* was created, and *favicon.ico*
![](blob:chrome-extension%3A//cdneggiaafcmelgcaiihmhiagieomgfj/26435c20-88f2-4846-a171-6eae22ebf9b7)

In My Views/Shared, _HeadImagePartial was nicely created.
![](blob:chrome-extension%3A//cdneggiaafcmelgcaiihmhiagieomgfj/d089b278-519d-492c-a51b-c2c7f2a94c97)

Now I simply go to my _Layout.cshtml to link this PartialView.
![](blob:chrome-extension%3A//cdneggiaafcmelgcaiihmhiagieomgfj/685254a1-ff03-4198-9982-47c35607ebcc)


### Now I dont' have to worry about any of these for rest of my project.
I can simply work with faviconsource.png and settings.json (and maybe template files), and project will be up to date everytime I build.

> **NOTE **
> When a new file was created as part of this build, it will not be included in your project by default.
> (notice the dotted icons in above images)
> Click on ***Show All Files *** in your Solution Explorer to see excluded files, right click on them and click **Add to Project**.
> &nbsp;
> Any **Updated** files will however, remain in your project.
