#!/usr/bin/env sh

# Tool checks
echo "Checking if cargo and rustc installed."
if ! command -v cargo || ! command -v rustc
then
  # Check if cargo/rustc installed
  echo "cargo/rustc command is missing. Check if you have rust installed. If not press Y to install rustup, if you want install your self press A."
  echo "Install Rustup? (Y)es/(A)bort"
  read -r ri_decision
  if [ $ri_decision = "Y" ];
  then
    curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
  elif [ $ri_decision = "A" ]; then
    exit
  fi
else
  # check if dotnet installed
  echo "Checking if dotnet installed."
  if ! command -v dotnet
  then
    echo "dotnet not installed. Please install it manually. aborting..."
    exit
  fi
  
  # make output directory
  if ! [ -d ./out ]
  then
    mkdir "out"
  fi
  
  # build launcher
  cd ./Tiels || (echo "cd failed. Does ./Tiels/ exist?" && exit)
  cargo build --release # --out-dir is unstable.
  cp ./target/release/Tiels ../out
  
  # build application
  cd ../Avalonia.Tiels || (echo "cd failed. Does ./Avalonia.Tiels/ exist?" && exit)
  dotnet build -o ../out
  
  echo "Output in ./out"
fi
