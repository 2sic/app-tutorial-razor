<img loading="lazy" src="app-icon.png" align="right" width="200px">

# Razor Tutorials for .net CMSs

> This is a [2sxc](https://2sxc.org) App for [DNN ☢️](https://www.dnnsoftware.com/) and [Oqtane 💧](https://www.oqtane.org/)

It contains around 500 snippets of code, which is executed when you look at it.
This guarantees that the code actually works.

You'll find a live, running copy of this tutorial on <https://go.2sxc.org/tut>.

You can also download the latest copy of this App directly from the [releases](./releases) and install it in a DNN with 2sxc, to play around with the code yourself.

| Aspect              | Status | Comments or Version |
| ------------------- | :----: | ------------------- |
| 2sxc                | ✅     | requires 2sxc v20.00                                                          |
| Dnn                 | ✅     | For v9.11.02+                                                                 |
| Oqtane 2            | ✅     | Requires v6.00+                                                               |
| No jQuery           | ✅     |                                                                               |
| Dnn Demo            | ✅     | See [Dnn Razor Tutorial](https://2sxc.org/dnn-tutorials/en/razor)             |
| Oqtane Demo         | ✅     | See [Oqtane Razor Tutorial](https://blazor-cms.org/oqtane-tutorials)          |
| Install Checklist   | ➖     | Just get from [releases](https://github.com/2sic/app-tutorial-razor/releases) |
| Source & License    | ✅     | included, ISC/MIT license                                                     |
| App Catalog         | ✅     | See [app catalog](https://2sxc.org/en/apps/app/dnn-razor-tutorial)            |
| Screenshots         | ✅     | See [app catalog](https://2sxc.org/en/apps/app/dnn-razor-tutorial)            |
| Best Practices      | ✅     | Uses v16.00 conventions                                                       |
| Bootstrap 3         | ✔️     |                                                                               |
| Bootstrap 4         | ✔️     | Works, but not optimized                                                      |
| Bootstrap 5         | ✅     | Works, optimized                                                              |

## Contribute

Feel free to contribute to this app, please just coordinate it w/iJungleboy.

[//]: # (## Customize the App not needed, so commented out)

## History

### 2025-11-16

1. 2dm: Improved sample with Template Delegates - added named tuples; much nicer

### 2025-09-18

1. 2dm: Added url protection to home and tutorial page, to stop crawlers from generating infinite urls
    1. If the home page has parameters, it will show a 404 since this is not expected
    1. If the tutorial page has a parameter with `=tut` it will show a 404 since this is not expected, it should always be `tut=...`

### v20 (2025-05+)

* Added Tutorials for [Static Assets Retrieval with App Query](https://2sxc.org/dnn-tutorials/en/razor/tut/data-app-assets)
  * Get Files via App Query
  * Get File via App Query
* Updated [URL Parameters Tutorials](https://2sxc.org/dnn-tutorials/en/razor/tut/code-link-parameters-modify)
  * Toggle()
  * Remove()
  * Filter()
  * Flush()
  * ContainsKey(Key)
  * Get(Key)
* Added Tutorials for [new Toolbar Features](https://2sxc.org/dnn-tutorials/en/razor/tut/ui-pickers-v20)
  * Tweak Notes
  * Audience Access
  * Edition Access/ Switching
* Extended User Service Tutorials with [App Query Fetching](https://2sxc.org/dnn-tutorials/en/razor/tut/get-users-appquery)
  * Get Users via App Query
  * Get Roles via App Query
* Added Tutorials for new [User Service](https://2sxc.org/dnn-tutorials/en/razor/tut/userservice)
  * Get Current User
  * Get Registered Users
  * Get Roles
  * Get User by Property
  * Get Users By Role

### v17 - v19

Lots of small changes, not detailed here

### v16

* v16.07.01 2023-09-26
  * reorganized so the snippets are all in the `/tutorials` folder
  * removed unused images
  * tested on Oqtane
* v16.07.00 2023-09-25
  * Restructured the entire system so each Snip is standalone in an own file
  * Build new infrastructure for Snips to exist in variants
  * Almost all infrastructure code is now typed
  * Added more than 100 tutorials for Typed
* v16.00.00 2023-05
  * Removed _ from Filenames
  * Enhanced Kit.Image with `imgAltFallback`
  * Replaced turnOn Tag with `Kit.Page.TurnOn`

### v12 - v14

* v14.07.05
  * Migrated the app to the 14.07 best practices
  * Changed the toolbar tutorials to also show the new IToolbarBuilder
  * Added a CmsContext tutorial
* v13.01
  * Enabled data-optimizations
  * Improved home
  * Added tutorial navigation
  * Added Oqtane support
  * Added IScrub tutorials
* v12.11 2021-12
  * Added new tutorials for turnOn
* v12.05 2021-10
  * Upgraded everything to also run in Oqtane (except for Dnn specific examples)
  * Using the latest & greatest best-practices of 2sxc 12.05
