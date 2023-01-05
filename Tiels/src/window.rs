use iced::widget::{button, column, text};
use iced::{Alignment, Element, Sandbox, Settings};

pub(crate) fn create_window() -> iced::Result {
    ErrorWindow::run(Settings::default())
}

struct ErrorWindow {
    title : String
}

#[derive(Debug, Clone, Copy)]
enum Message {
    CloseApplication
}

impl Sandbox for ErrorWindow {
    type Message = Message;

    fn new() -> Self {
        Self { title: "".to_string() }
    }

    fn title(&self) -> String {
        String::from("ErrorWindow - Iced")
    }

    fn update(&mut self, message: Message) {
        match message {
            Message::CloseApplication => {
            }
        }
    }

    fn view(&self) -> Element<Message> {
        column![
            button("Increment").on_press(Message::CloseApplication),
        ]
            .padding(20)
            .align_items(Alignment::Center)
            .into()
    }
}
