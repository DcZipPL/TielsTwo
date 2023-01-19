## Project structure
### Avalonia.Tiels
Main application (C#)

### Tiels
Launcher / Updater and Error Handler written in rust

### build
Python build scripts and build files
- build.py : Build project
- publish.py : Build project with release flag
- prepare.py : Shared code for build.py and publish.py
- svg2font.py : Creates iconfont
- include.toml : Lucide font glyph names to download and add to iconfont
- default.iconfont_metadata.json : Metadata for font creation

### build/out
Build output

### assets
Global assets used by build script, Avalonia.Tiels and Tiels.<br>
All files in ./assets will be copied by python script to corresponding source.

