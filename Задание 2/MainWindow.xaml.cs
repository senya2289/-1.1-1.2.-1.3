﻿using System.Windows;

namespace Задание_2
{
    public partial class MainWindow : Window
    {
        private Notification _notification;

        public MainWindow()
        {
            InitializeComponent();
            _notification = new Notification();

            // Регистрация обработчиков событий
            _notification.OnMessageSent += MessageSentHandler;
            _notification.OnCallMade += CallMadeHandler;
            _notification.OnEmailSent += EmailSentHandler;
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageTextBox.Text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                _notification.SendMessage(message);
            }
            else
            {
                MessageBox.Show("Введите текст сообщения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MakeCallButton_Click(object sender, RoutedEventArgs e)
        {
            string phoneNumber = CallTextBox.Text;
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                _notification.MakeCall(phoneNumber);
            }
            else
            {
                MessageBox.Show("Введите номер телефона.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SendEmailButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            if (!string.IsNullOrWhiteSpace(email))
            {
                _notification.SendEmail(email);
            }
            else
            {
                MessageBox.Show("Введите адрес электронной почты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Обработчики событий
        private void MessageSentHandler(string message)
        {
            NotificationLogTextBlock.Text = $"Сообщение отправлено: {message}";
        }

        private void CallMadeHandler(string phoneNumber)
        {
            NotificationLogTextBlock.Text = $"Звонок совершен на номер: {phoneNumber}";
        }

        private void EmailSentHandler(string email)
        {
            NotificationLogTextBlock.Text = $"Письмо отправлено на: {email}";
        }
    }
}