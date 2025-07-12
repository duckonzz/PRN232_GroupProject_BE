using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Core.Config
{
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new(new VnPayCompare());
        private readonly SortedList<string, string> _responseData = new(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                _requestData[key] = value;
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                _responseData[key] = value;
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            _responseData.Remove("vnp_SecureHash");
            _responseData.Remove("vnp_SecureHashType");

            var rawData = GetQueryString(_responseData);
            var myChecksum = ComputeHmacSha512(secretKey, rawData);
            return myChecksum.Equals(inputHash, StringComparison.OrdinalIgnoreCase);
        }

        public string CreateRequestUrl(string baseUrl, string secretKey)
        {
            var rawData = GetQueryString(_requestData);
            var secureHash = ComputeHmacSha512(secretKey, rawData);
            return $"{baseUrl}?{rawData}&vnp_SecureHash={secureHash}";
        }

        private string GetQueryString(SortedList<string, string> data)
        {
            var sb = new StringBuilder();
            foreach (var kv in data)
            {
                sb.Append($"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}&");
            }
            return sb.ToString().TrimEnd('&');
        }

        private string ComputeHmacSha512(string key, string inputData)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using var hmac = new HMACSHA512(keyBytes);
            var hash = hmac.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y) => string.CompareOrdinal(x, y);
    }
}

