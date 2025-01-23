using HostelFinder.Domain.Entities;

namespace HostelFinder.Domain.Common.Constants
{
    public class EmailConstants
    {
        public static string BodyActivationEmail(string email) =>
            @"
                <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Password Reset</title>
                    <style>
                        /* Reset styles */
                        body, html {
                            margin: 0;
                            padding: 0;
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                        }
                        /* Container styles */
                        .container {
                            max-width: 600px;
                            margin: 20px auto;
                            padding: 20px;
                            border: 1px solid #ccc;
                            border-radius: 10px;
                            background-color: #f9f9f9;
                        }
                        /* Heading styles */
                        h1 {
                            font-size: 24px;
                            text-align: center;
                            color: #333;
                        }
                        /* Paragraph styles */
                        p {
                            margin-bottom: 20px;
                            color: #666;
                        }
                        /* Button styles */
                        .btn {
                            display: inline-block;
                            padding: 10px 20px;
                            background-color: #007bff;
                            color: #fff;
                            text-decoration: none;
                            border-radius: 5px;
                        }
                        /* Footer styles */
                        .footer {
                            margin-top: 20px;
                            text-align: center;
                            color: #999;
                        }
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <p>Hello,</p>
                        <p>Welcome to Base Project. Thank you for using our servicesđe</p>
                        <p>To experience the service, please activate your account. Click the button below:</p>
                        <p><a href=""http://localhost:5000/Home/Resetpassword?userId=2}"" class=""btn"">Active Account</a></p>
                        <p>If you have any questions or need assistance, please contact our support team.</p>
                        <p>Thank you,</p>
                        <p>The Support Team</p>
                        <div class=""footer"">
                            <p>This is an automated message. Please do not reply.</p>
                        </div>
                    </div>
                </body>
                </html>
              ";

        public static string BodyResetPasswordEmail(User user, string email, string newPassword)
        {
            return $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    margin: 0;
                    padding: 0;
                    background-color: #f6f6f6;
                }}
                .container {{
                    max-width: 600px;
                    margin: 0 auto;
                    background-color: #ffffff;
                    padding: 20px;
                    border-radius: 8px;
                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                }}
                h2 {{
                    color: #333333;
                }}
                p {{
                    color: #666666;
                    line-height: 1.6;
                }}
                .highlight {{
                    font-weight: bold;
                    color: #e74c3c;
                }}
                a.button {{
                    display: inline-block;
                    padding: 10px 20px;
                    margin-top: 20px;
                    background-color: #28a745;
                    color: #ffffff;
                    text-decoration: none;
                    font-weight: bold;
                    border-radius: 5px;
                }}
                a.button:hover {{
                    background-color: #218838;
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 12px;
                    color: #999999;
                    text-align: center;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Thông báo mật khẩu mới</h2>
                <p>Xin chào, <span class='highlight'>{user.FullName}</span></p>
                <p>Chúng tôi đã nhận được yêu cầu thay đổi mật khẩu cho tài khoản của bạn với email <strong>{email}</strong>.</p>
                <p>Đây là mật khẩu mới của bạn:</p>
                <p><span class='highlight'>Tên đăng nhập: {user.Username}</span></p>
                <p><span class='highlight'>Mật khẩu mới: {newPassword}</span></p>
                <p>Vui lòng sử dụng mật khẩu mới để đăng nhập vào tài khoản của bạn. Bạn có thể thay đổi mật khẩu sau khi đăng nhập vào phần cài đặt tài khoản của mình.</p>
                <p>Nếu bạn không yêu cầu thay đổi mật khẩu này, vui lòng liên hệ ngay với chúng tôi để được hỗ trợ.</p>
                <p>Trân trọng,<br/>Đội ngũ hỗ trợ khách hàng</p>
                <div class='footer'>
                    <p>© 2024 Phongtro247.</p>
                </div>
            </div>
        </body>
        </html>";
        }


        public static string BodyRegisterMembership(User user, Membership membership)
        {
            return $@"
        <!DOCTYPE html>
        <html lang='vi'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Thông báo đăng ký thành công</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    color: #333;
                    background-color: #f7f7f7;
                    margin: 0;
                    padding: 0;
                }}
                .email-container {{
                    width: 100%;
                    max-width: 600px;
                    margin: 0 auto;
                    background-color: #ffffff;
                    padding: 20px;
                    border-radius: 8px;
                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                }}
                .email-header {{
                    background-color: #2c3e50;
                    color: #ffffff;
                    padding: 10px;
                    text-align: center;
                    border-radius: 8px 8px 0 0;
                }}
                .email-body {{
                    padding: 20px;
                    line-height: 1.6;
                }}
                .email-footer {{
                    text-align: center;
                    font-size: 12px;
                    color: #777;
                    margin-top: 20px;
                }}
                .btn {{
                    display: inline-block;
                    padding: 10px 20px;
                    background-color: #3498db;
                    color: #ffffff;
                    text-decoration: none;
                    border-radius: 5px;
                    text-align: center;
                    margin-top: 20px;
                }}
                .btn:hover {{
                    background-color: #2980b9;
                }}
            </style>
        </head>
        <body>
            <div class='email-container'>
                <div class='email-header'>
                    <h2>PhongTro247 - Thông báo đăng ký thành công</h2>
                </div>
                <div class='email-body'>
                    <p>Kính gửi khách hàng <strong>{user.Email}</strong>,</p>
                    <p>Chúng tôi xin thông báo rằng bạn đã đăng ký thành công gói hội viên <strong>{membership.Name}</strong> tại PhongTro247.</p>
                    <p><strong>Chi tiết gói sử dụng:</strong></p>
                    <p>{membership.Description}</p>
                    <p>Chúc bạn có những trải nghiệm tuyệt vời với dịch vụ của chúng tôi!</p>
                    <p>
                        <a href='#' class='btn'>Quản lý gói hội viên của bạn</a>
                    </p>
                </div>
                <div class='email-footer'>
                    <p>Trân trọng, <br>PhongTro247 Team</p>
                    <p>Địa chỉ: Hòa Lạc, Hà Nội</p>
                </div>
            </div>
        </body>
        </html>
        ";
        }

        public const string SUBJECT_REGISTER_MEMBERSHIP = "Đăng ký thành công gói hội viên";
        public const string SUBJECT_RESET_PASSWORD = "Reset Password";
        public const string SUBJECT_ACTIVE_ACCOUNT = "Active Email";
    }
}