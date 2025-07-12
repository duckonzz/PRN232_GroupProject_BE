using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Config;
using GenderHealthCare.ModelViews.VNPayModels;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web;

public class VnPayService : IVnPayService
{
    private readonly IConfiguration _config;

    public VnPayService(IConfiguration config)
    {
        _config = config;
    }

    public string CreatePaymentUrl(VnPayPaymentRequest request, string ipAddress)
    {
        var lib = new VnPayLibrary();

        lib.AddRequestData("vnp_Version", _config["Vnpay:Version"]);
        lib.AddRequestData("vnp_Command", _config["Vnpay:Command"]);
        lib.AddRequestData("vnp_TmnCode", _config["Vnpay:TmnCode"]);
        lib.AddRequestData("vnp_Amount", (request.Amount * 100).ToString());
        lib.AddRequestData("vnp_CurrCode", _config["Vnpay:CurrCode"]);
        lib.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());
        lib.AddRequestData("vnp_OrderInfo", $"{request.UserId}_{request.ServiceId}");

        lib.AddRequestData("vnp_OrderType", "other");
        lib.AddRequestData("vnp_Locale", _config["Vnpay:Locale"]);
        lib.AddRequestData("vnp_ReturnUrl", _config["Vnpay:PaymentBackReturnUrl"]);
        lib.AddRequestData("vnp_IpAddr", ipAddress);
        lib.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
        lib.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

        string paymentUrl = lib.CreateRequestUrl(_config["Vnpay:BaseUrl"], _config["Vnpay:HashSecret"]);
        return paymentUrl;
    }
}
