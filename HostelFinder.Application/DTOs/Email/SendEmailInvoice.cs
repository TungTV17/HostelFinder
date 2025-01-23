using HostelFinder.Application.DTOs.InVoice.Responses;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.DTOs.Email;

public class SendEmailInvoice
{
 public static string BodyInvoiceEmail(User user, InvoiceResponseDto invoice)
{
    var invoiceDate = DateTime.Now.ToString("dd/MM/yyyy");
    var billingPeriod = $"{invoice.BillingMonth}/{invoice.BillingYear}";
    var invoiceDetailsRows = string.Empty;

    foreach (var detail in invoice.InvoiceDetails)
    {
        invoiceDetailsRows += $@"
            <tr>
                <td>{detail.ServiceName}</td>
                <td style='text-align: right;'>{detail.UnitCost:#,##0} VND</td>
                <td style='text-align: right;'>{detail.ActualCost:#,##0} VND</td>
            </tr>";
    }
    
    string statusColor = invoice.IsPaid ? "green" : "red";
    string statusText = invoice.IsPaid ? "Đã Thanh Toán" : "Chưa Thanh Toán";
    string statusHtml = $"<span style='color: {statusColor}; font-weight: bold;'>{statusText}</span>";

    string paymentSection = string.Empty;
    if (!invoice.IsPaid)
    {
        paymentSection = $@"
            <div class='payment-section'>
                <h3>Thông Tin Thanh Toán</h3>
                <div class='payment-info'>
                    <div class='bank-details'>
                        <p><strong>Ngân hàng:</strong> {user.BankName}</p>
                        <p><strong>Số tài khoản:</strong> {user.AccountNumber}</p>
                        <p><strong>Số tiền:</strong> {invoice.TotalAmount:#,##0} VND</p>
                        <p><strong>Nội dung chuyển khoản:</strong> THANHTOAN_{invoice.BillingMonth}{invoice.BillingYear}</p>
                    </div>
                    <div class='qr-code'>
                        <img src='{user.QRCode}' alt='QR Code Thanh Toán' style='max-width: 200px;'>
                        <p class='qr-note'>Quét mã QR để thanh toán nhanh</p>
                    </div>
                </div>
            </div>";
    }

    return $@"
        <!DOCTYPE html>
        <html lang='vi'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Hóa Đơn Phòng Trọ</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f7f7f7;
                    margin: 0;
                    padding: 0;
                }}
                .email-container {{
                    width: 100%;
                    max-width: 700px;
                    margin: 0 auto;
                    background-color: #ffffff;
                    padding: 20px;
                    border-radius: 8px;
                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                }}
                .header {{
                    background-color: #2c3e50;
                    color: #ffffff;
                    padding: 15px;
                    text-align: center;
                    border-radius: 8px 8px 0 0;
                }}
                .body {{
                    padding: 20px;
                    line-height: 1.6;
                }}
                .footer {{
                    text-align: center;
                    font-size: 12px;
                    color: #777777;
                    margin-top: 20px;
                    padding-top: 20px;
                    border-top: 1px solid #dddddd;
                }}
                table {{
                    width: 100%;
                    border-collapse: collapse;
                    margin: 20px 0;
                }}
                th, td {{
                    padding: 12px;
                    border: 1px solid #dddddd;
                }}
                th {{
                    background-color: #f2f2f2;
                }}
                .total {{
                    font-weight: bold;
                }}
                .payment-section {{
                    margin: 30px 0;
                    padding: 20px;
                    background-color: #f8f9fa;
                    border-radius: 8px;
                }}
                .payment-info {{
                    display: flex;
                    justify-content: space-between;
                    align-items: flex-start;
                    flex-wrap: wrap;
                    gap: 20px;
                }}
                .bank-details {{
                    flex: 1;
                    min-width: 250px;
                }}
                .qr-code {{
                    text-align: center;
                }}
                .qr-note {{
                    margin-top: 10px;
                    font-size: 14px;
                    color: #666;
                }}
                .status-badge {{
                    display: inline-block;
                    padding: 5px 10px;
                    border-radius: 4px;
                    font-weight: bold;
                }}
                @media screen and (max-width: 600px) {{
                    .payment-info {{
                        flex-direction: column;
                    }}
                    .bank-details, .qr-code {{
                        width: 100%;
                    }}
                }}
            </style>
        </head>
        <body>
            <div class='email-container'>
                <div class='header'>
                    <h2>PhongTro247 - Hóa Đơn Phòng Trọ</h2>
                </div>
                <div class='body'>
                    <p>Kính gửi quý khách hàng,</p>
                    <p>Dưới đây là hóa đơn cho kỳ thanh toán <strong>{billingPeriod}</strong>:</p>
                    <p><strong>Ngày Lập Hóa Đơn:</strong> {invoiceDate}</p>
                    <table>
                        <thead>
                            <tr>
                                <th>Dịch Vụ</th>
                                <th>Đơn Giá</th>
                                <th>Thành Tiền</th>
                            </tr>
                        </thead>
                        <tbody>
                            {invoiceDetailsRows}
                            <tr>
                                <td colspan='2' class='total'>Tổng Cộng</td>
                                <td style='text-align: right;' class='total'>{invoice.TotalAmount:#,##0} VND</td>
                            </tr>
                        </tbody>
                    </table>
                    <p><strong>Trạng Thái:</strong> {statusHtml}</p>
                    {paymentSection}
                    <p>Vui lòng thanh toán hóa đơn tháng <strong>{billingPeriod}</strong> để tránh bị phạt trễ hạn.</p>
                    <p>Trân trọng cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
                </div>
                <div class='footer'>
                    <p>© 2024 PhongTro247. All rights reserved.</p>
                    <p>Địa chỉ: Hòa Lạc, Hà Nội</p>
                </div>
            </div>
        </body>
        </html>";
}

        public static string BodyInvoiceSuccessEmail(InvoiceResponseDto invoice)
    {
    var invoiceDate = DateTime.Now.ToString("dd/MM/yyyy");
    var billingPeriod = $"{invoice.BillingMonth}/{invoice.BillingYear}";
    var invoiceDetailsRows = string.Empty;

    foreach (var detail in invoice.InvoiceDetails)
    {
        invoiceDetailsRows += $@"
            <tr>
                <td>{detail.ServiceName}</td>
                <td style='text-align: right;'>{detail.UnitCost:N0} VND</td>
                <td style='text-align: right;'>{detail.ActualCost:N0} VND</td>
            </tr>";
    }
    
    string statusColor = invoice.IsPaid ? "green" : "red";
    string statusText = invoice.IsPaid ? "Đã Thanh Toán" : "Chưa Thanh Toán";
    string statusHtml = $"<span style='color: {statusColor}; font-weight: bold;'>{statusText}</span>";


    return $@"
        <!DOCTYPE html>
        <html lang='vi'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Hóa Đơn Phòng Trọ</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f7f7f7;
                    margin: 0;
                    padding: 0;
                }}
                .email-container {{
                    width: 100%;
                    max-width: 700px;
                    margin: 0 auto;
                    background-color: #ffffff;
                    padding: 20px;
                    border-radius: 8px;
                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                }}
                .header {{
                    background-color: #2c3e50;
                    color: #ffffff;
                    padding: 10px;
                    text-align: center;
                    border-radius: 8px 8px 0 0;
                }}
                .body {{
                    padding: 20px;
                    line-height: 1.6;
                }}
                .footer {{
                    text-align: center;
                    font-size: 12px;
                    color: #777777;
                    margin-top: 20px;
                }}
                table {{
                    width: 100%;
                    border-collapse: collapse;
                    margin-top: 20px;
                }}
                th, td {{
                    padding: 10px;
                    border: 1px solid #dddddd;
                }}
                th {{
                    background-color: #f2f2f2;
                }}
                .total {{
                    font-weight: bold;
                }}
            </style>
        </head>
        <body>
            <div class='email-container'>
                <div class='header'>
                    <h2>PhongTro247 - Thông tin hóa đơn Phòng Trọ</h2>
                </div>
                <div class='body'>
                    <p>Kính gửi quý khách hàng,</p>
                    <p>Thông tin chi tiêt hóa đơn thanh toán tháng <strong>{billingPeriod}</strong>:</p>
                    <p><strong>Ngày Lập Hóa Đơn:</strong> {invoiceDate}</p>
                    <p><strong>Trạng Thái:</strong> {statusHtml}</p>
                    <p><strong>Số tiền:</strong> {invoice.TotalAmount:#,##0} VNĐ</p>
                    <p><strong> Hình thức thành toán : {invoice.FormOfTransfer} </strong></p>
                    <table>
                        <thead>
                            <tr>
                                <th>Dịch Vụ</th>
                                <th>Đơn Giá</th>
                                <th>Thành Tiền</th>
                            </tr>
                        </thead>
                        <tbody>
                            {invoiceDetailsRows}
                            <tr>
                                <td colspan='2' class='total'>Tổng Cộng</td>
                                <td style='text-align: right;' class='total'>{invoice.TotalAmount:#,##0} VND</td>
                            </tr>
                        </tbody>
                    </table>
                    <p>Trân trọng cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
                </div>
                <div class='footer'>
                    <p>© 2024 PhongTro247. All rights reserved.</p>
                    <p>Địa chỉ: Hòa Lạc, Hà Nội</p>
                </div>
            </div>
        </body>
        </html>";
    }
    public const string SUBJECT_INVOICE = $"Hóa Đơn Phòng Trọ Của Bạn Từ PhongTro247";
    public const string SUBJECT_INVOICE_SUCCESS = $"Thông Tin Hóa Đơn Thanh Toán Phòng Trọ Từ PhongTro247";
}