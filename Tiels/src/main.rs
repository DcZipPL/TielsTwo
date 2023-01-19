mod window;

use std::env;
use std::process::Command;

fn main() {
    let args = env::args().collect::<Vec<String>>();
    if args.len() > 1 {
        match args[1].as_str() {
            "update" => {},
            "error" => {
                show_error(args);
            },
            _ => {}
        }
    }
}

fn show_error(args : Vec<String>) {
    if args.len() > 3 {
        window::create_window().expect("Couldn't create a window");
    }
}
