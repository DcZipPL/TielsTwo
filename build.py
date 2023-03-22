import argparse
import json
import os
import platform
import re
import shutil
import sys
import urllib.request
import urllib.error
from typing import final
from subprocess import Popen

ICONS_URL: final(str) = "https://raw.githubusercontent.com/lucide-icons/lucide/main/icons/"
ICONS_PATH: final(str) = "./assets/icons-remote/"
ICONS_LOCAL_PATH: final(str) = "./assets/icons/"

WRAPPER: final(str) = rf"""
@echo off
echo Configuring the system path to add FontForge...
set FF=%~dp0
set "PYTHONHOME=%FF%"

if not defined FF_PATH_ADDED (
set "PATH=%FF%;%FF%\bin;%PATH:"=%"
set FF_PATH_ADDED=TRUE
)

for /F "tokens=* USEBACKQ" %%f IN (`dir /b "%FF%lib\python*"`) do (
set "PYTHONPATH=%FF%lib\%%f"
)

cd "{os.path.dirname(os.path.realpath(__file__))}"
ffpython "{os.path.realpath(__file__)}" fontforge
"""


def __to_camel_case(text: str):
	temp = text.split('-')
	return ''.join(ele.title() for ele in temp[0:])


def prepare_icons():
	# Download used icons. We don't want to download all, it will increase binary size.
	with open("./assets/include.toml", "rb") as f:
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
		if not os.path.isdir(ICONS_PATH):
			os.mkdir(ICONS_PATH)

		shutil.copy("./assets/default.iconfont_metadata.json", "./assets/iconfont_metadata.json")
		cs_const_lines: str = ""
		ii: int = 1046160
		li: int = 1046160 + 256
		# Local icons
		for local_icon in toml_data.get("local_icons"):
			shutil.copy(f"{ICONS_LOCAL_PATH}{local_icon}.svg", f"{ICONS_PATH}{local_icon}.svg")
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
					with open(f"{ICONS_PATH}{icon}.svg", "r", encoding='utf-8') as i:
						i_st = i.read().replace("stroke-width=\"2\"", "stroke-width=\"1.5\"")
					with open(f"{ICONS_PATH}{icon}.svg", "w", encoding='utf-8') as i:
						i.write(i_st)
				else:
					print(f"Skipping {icon}.svg, already exists!")

				cs_const_lines = autogen_icons_to_files(ii, icon, cs_const_lines)
				ii += 1
			except urllib.error.HTTPError:
				print(f"Couldn't download {icon}.svg")
		print("Writing all icons to C# file...")
		add_icons_to_cs(cs_const_lines)


def generate_icons(fontforge_path: str):
	prepare_icons()

	# Make icon font
	print("Make iconfont...")
	try:
		from assets import svg2font
		svg2font.main("iconfont_metadata.json")
	except ModuleNotFoundError as mex:
		print(mex)
		print("Fontforge python extension not found!")
		try:
			if platform.system() == "Windows":
				if fontforge_path == "":
					fontforge_path = input("If you don't have FontForge installed, please download it from "
										   "https://fontforge.org/en-US/downloads/windows/ and install it.\n"
										   "! If you have it installed then please enter the path to FontForge directory.\n"
										   "! You can also set the path using --fontforge-path argument.\n"
										   "! Path: ")
				if not os.path.exists(f"{fontforge_path}/fontforge-python.bat"):
					print("Creating workaround for fontforge python extension...")
					with open(f"{fontforge_path}/fontforge-python.bat", "w", encoding='utf-8') as fontforge_wrapper_file:
						fontforge_wrapper_file.write(WRAPPER)

			elif platform.system() == "Linux":
				print("Please install fontforge python extension using `sudo apt install fontforge python-fontforge`")
				exit(1)
			elif platform.system() == "Darwin":  # macOS
				print("Please install fontforge python extension using `brew install fontforge`")
				exit(1)
			else:
				print("Unknown OS! Install fontforge python extension manually.")
				exit(1)

			p = Popen(f"{fontforge_path}/fontforge-python.bat")
			stdout, stderr = p.communicate()
		except OSError as ex:
			print(ex)
			exit(ex.errno)


def autogen_icons_to_files(char_index: int, icon_name: str, cs_lines_to_expand: str):
	# Add glyph to iconfont metadata
	with open(f"assets/iconfont_metadata.json", "r", encoding='utf-8') as metadata_file:
		loaded_metadata = json.load(metadata_file)
		loaded_metadata["glyphs"][str(char_index)] = {"src": icon_name + ".svg"}
	with open(f"assets/iconfont_metadata.json", "w", encoding='utf-8') as metadata_file:
		metadata_file.write(json.dumps(loaded_metadata))

	# Add icon to C# class
	cs_lines_to_expand += f"""\n\tpublic static readonly string {
	__to_camel_case(icon_name).replace("2", "Alt")
	} = \"{chr(char_index)}\";"""
	return cs_lines_to_expand


def add_icons_to_cs(cs_consts: str):
	temp_class: str
	with open("./Avalonia.Tiels/Classes/Icons.cs", "r", encoding='utf-8') as c:
		temp_class = c.read()

	temp_class = re.sub(r"(?<=//!a).*(?=//!a)", cs_consts + "\n\t", temp_class, flags=re.DOTALL)

	with open("./Avalonia.Tiels/Classes/Icons.cs", "w", encoding='utf-8') as c:
		c.write(temp_class)


def is_installed(name: str):
	if shutil.which(name) is None:
		print(f"{name} not found! Aborting!")
		exit(21)


if __name__ == "__main__":
	parser = argparse.ArgumentParser(
		prog='Tiels build script',
		description='Build script for Tiels written in Python')

	parser.add_argument('action', choices=['debug', 'release', 'icons', 'fontforge'])  # positional argument
	parser.add_argument('--fontforge-path', default="")  # optional argumentf flag

	args = parser.parse_args()

	is_installed("cargo")
	is_installed("dotnet")

	if args.action == 'icons':
		generate_icons(args.fontforge_path)
	elif args.action == 'fontforge':
		from assets import svg2font
		svg2font.main("assets/iconfont_metadata.json")
