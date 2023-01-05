import urllib.request
import urllib.error
import shutil
import os
import sys
import subprocess
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

        # Download icons
        for icon in toml_data.get("icons"):
            try:
                urllib.request.urlretrieve(ICONS_URL + icon + ".svg", ICONS_PATH + icon + ".svg")
                print("Downloaded " + icon + ".svg")
            except urllib.error.HTTPError:
                print("Couldn't download " + icon + ".svg")
    # Other deps


def create_output():
    if not os.path.isdir("out"):
        os.mkdir("out")


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
    print("Starting build tasks...")
    release_str: str = ""
    if release:
        release_str = " --release"
    exitcode = subprocess.call("cd ./Tiels && cargo build"+release_str+" && cp ./target/release/Tiels ../out",
                               shell=True)
    if exitcode != 0:
        print("Compilation error occurred! Aborting!")
        exit(exitcode)
