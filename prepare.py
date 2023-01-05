import urllib.request
import urllib.error
import shutil
import os
import sys
import subprocess
import platform
from typing import final


ICONS_URL: final(str) = "https://raw.githubusercontent.com/lucide-icons/lucide/main/icons/"
ICONS_PATH: final(str) = "./Avalonia.Tiels/Assets/Icons/out/"


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
        for icon in toml_data.get("icons"):
            try:
                if not os.path.exists(ICONS_PATH + icon + ".svg"):
                    urllib.request.urlretrieve(ICONS_URL + icon + ".svg", ICONS_PATH + icon + ".svg")
                    print("Downloaded " + icon + ".svg")
                else:
                    print("Skipping " + icon + ".svg, already exists!")
            except urllib.error.HTTPError:
                print("Couldn't download " + icon + ".svg")
    # Other deps


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
    print("Starting build tasks (1/2)...")
    release_flag: str = ""

    # Launcher/Updater
    if release:
        release_flag = " --release"
    exitcode = subprocess.call("cd ./Tiels && cargo build"+release_flag,
                               shell=True)  # --out-dir is unstable.
    # Exit script if it failed
    if exitcode != 0:
        print("Compilation error occurred! Aborting!")
        exit(exitcode)
    
    # Copy compiled executable to output directory
    print("Copying compiled binary...")
    cp_ext: str = ""
    cp_dr: str = "debug"
    if platform.system() == "Windows":
        cp_ext = ".exe"
    if release:
        cp_dr = "release"
    shutil.copy("./Tiels/target/"+cp_dr+"/Tiels"+cp_ext, "./out/Tiels"+cp_ext)

    # Main program
    print("Starting build tasks (2/2)...")
    release_flag = "build"
    if release:
        release_flag = "publish"
    exitcode = subprocess.call("cd ./Avalonia.Tiels && dotnet "+release_flag+" -o ../out/bin",
                               shell=True)

    # Exit script if it failed
    if exitcode != 0:
        print("Compilation error occurred! Aborting!")
        exit(exitcode)
