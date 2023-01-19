import json
import urllib.request
import urllib.error
import shutil
import os
import re
import sys
import json
import subprocess
import platform
import svg2font
from typing import final


ICONS_URL: final(str) = "https://raw.githubusercontent.com/lucide-icons/lucide/main/icons/"
ICONS_PATH: final(str) = "./icons/"



def download_dependencies():
    # Download used icons. We don't want to download all, it will increase binary size.
    with open("include.toml", "rb") as f:
        toml_data: dict

        # Read include.toml file
        if sys.version_info[0] >= 3 and sys.version_info[1] >= 11:
            print("Using `tomllib` for toml parsing.")
            import tomllib
            toml_data = tomllib.load(f)
        else:
            print("Using outdated python version! Using `toml` package instead of standard `tomllib`")
            # Check if toml installed. if not then exit
            try:
                import toml
                toml_data = toml.loads(f.read().decode("utf-8"))
            except ModuleNotFoundError:
                print("`toml` package not installed! Aborting!")
                exit(137)

        # Check if icons exist already and download icons
        if not os.path.isdir("./icons"):
            os.mkdir("./icons")

        shutil.copy("default.iconfont_metadata.json", "iconfont_metadata.json")
        cs_const_lines: str = ""
        ii: int = 1046160
        li: int = 1046160 + 256
        # Local icons
        for local_icon in toml_data.get("local_icons"):
            shutil.copy(f"../assets/{local_icon}.svg", f"{ICONS_PATH}{local_icon}.svg")
            cs_const_lines = autogen_icons_to_files(li, local_icon, cs_const_lines)
            li += 1
        # Remote icons
        for icon in toml_data.get("icons"):
            try:
                if not os.path.exists(f"{ICONS_PATH}{icon}.svg"):
                    print(f"Downloading... {icon}.svg")
                    urllib.request.urlretrieve(f"{ICONS_URL}{icon}.svg", f"{ICONS_PATH}{icon}.svg")

                    # Make svg stroke smaller for icons
                    i_st: str
                    with open(f"{ICONS_PATH}{icon}.svg", "r") as i:
                        i_st = i.read().replace("stroke-width=\"2\"", "stroke-width=\"1.5\"")
                    with open(f"{ICONS_PATH}{icon}.svg", "w") as i:
                        i.write(i_st)
                else:
                    print(f"Skipping {icon}.svg, already exists!")

                cs_const_lines = autogen_icons_to_files(ii, icon, cs_const_lines)
                ii += 1
            except urllib.error.HTTPError:
                print(f"Couldn't download {icon}.svg")
        print("Writing all icons to C# file...")
        add_icons_to_cs(cs_const_lines)
    # Other deps


def autogen_icons_to_files(char_index: int, icon_name: str, cs_lines_to_expand: str):
    # Add glyph to iconfont metadata
    with open(f"iconfont_metadata.json", "r") as metadata_file:
        loaded_metadata = json.load(metadata_file)
        loaded_metadata["glyphs"][str(char_index)] = {"src": icon_name + ".svg"}
    with open(f"iconfont_metadata.json", "w") as metadata_file:
        metadata_file.write(json.dumps(loaded_metadata))

    # Add icon to C# class
    cs_lines_to_expand += f"""\n\tpublic static readonly string {
    __to_camel_case(icon_name).replace("2", "Alt")
    } = \"{chr(char_index)}\";"""
    return cs_lines_to_expand


def add_icons_to_cs(cs_consts: str):
    temp_class: str
    with open("../Avalonia.Tiels/Classes/Icons.cs", "r") as c:
        temp_class = c.read()

    temp_class = re.sub(r"(?<=\/\/!a).*(?=\/\/!a)", cs_consts+"\n\t", temp_class, flags=re.DOTALL)

    with open("../Avalonia.Tiels/Classes/Icons.cs", "w") as c:
        c.write(temp_class)


def create_output():
    if not os.path.isdir("out"):
        os.mkdir("out")
    if not os.path.isdir("out/bin"):
        os.mkdir("out/bin")


def check_buildtools():
    print("Checking required buildtools...")
    missing_execs: int = 0
    missing_execs += int(not __check_buildtool("cargo"))
    missing_execs += int(not __check_buildtool("rustc"))
    missing_execs += int(not __check_buildtool("dotnet"))
    return missing_execs == 0


def __check_buildtool(tool: str):
    print(tool + "... " + str(shutil.which(tool) is not None))
    return shutil.which(tool) is not None


def build(release: bool):
    # Make icon font
    print("Make iconfont...")
    svg2font.main("iconfont_metadata.json")

    # Launcher/Updater
    print("Starting build tasks (1/2)...")
    release_flag: str = ""

    if release:
        release_flag = " --release"
    exitcode = subprocess.call(f"cd ../Tiels && cargo build{release_flag}",
                               shell=True)  # --out-dir is unstable.
    # Exit script if it failed
    if exitcode != 0:
        print("Compilation error occurred! Aborting!")
        exit(exitcode)
    
    # Copy compiled executable to output directory
    if len(sys.argv) <= 1 or not sys.argv[1] == "no_out":
        print("Copying compiled binary...")
        cp_ext: str = ""
        cp_dr: str = "debug"
        if platform.system() == "Windows":
            cp_ext = ".exe"
        if release:
            cp_dr = "release"
        shutil.copy(f"../Tiels/target/{cp_dr}/Tiels{cp_ext}", f"./out/Tiels{cp_ext}")

    # Main program
    print("Starting build tasks (2/2)...")
    release_flag = "build"
    out_flag = ""
    if release:
        release_flag = "publish"
    if len(sys.argv) <= 1 or not sys.argv[1] == "no_out":
        out_flag = "-o ../build/out/bin/"
    exitcode = subprocess.call(f"cd ../Avalonia.Tiels && dotnet {release_flag} -c Release {out_flag}",
                               shell=True)

    # Exit script if it failed
    if exitcode != 0:
        print("Compilation error occurred! Aborting!")
        exit(exitcode)


def __to_camel_case(text: str):
    temp = text.split('-')
    return ''.join(ele.title() for ele in temp[0:])

print("It's a lib, use build.py or publish.py")