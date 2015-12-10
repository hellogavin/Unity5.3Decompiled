namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;

    internal class PurchaseResult : AssetStoreResultBase<PurchaseResult>
    {
        public string message;
        public int packageID;
        public Status status;

        public PurchaseResult(AssetStoreResultBase<PurchaseResult>.Callback c) : base(c)
        {
        }

        protected override void Parse(Dictionary<string, JSONValue> dict)
        {
            this.packageID = int.Parse(dict["package_id"].AsString());
            this.message = !dict.ContainsKey("message") ? null : dict["message"].AsString(true);
            JSONValue value4 = dict["status"];
            switch (value4.AsString(true))
            {
                case "basket-not-empty":
                    this.status = Status.BasketNotEmpty;
                    break;

                case "service-disabled":
                    this.status = Status.ServiceDisabled;
                    break;

                case "user-anonymous":
                    this.status = Status.AnonymousUser;
                    break;

                case "password-missing":
                    this.status = Status.PasswordMissing;
                    break;

                case "password-wrong":
                    this.status = Status.PasswordWrong;
                    break;

                case "purchase-declined":
                    this.status = Status.PurchaseDeclined;
                    break;

                case "ok":
                    this.status = Status.Ok;
                    break;
            }
        }

        public enum Status
        {
            BasketNotEmpty,
            ServiceDisabled,
            AnonymousUser,
            PasswordMissing,
            PasswordWrong,
            PurchaseDeclined,
            Ok
        }
    }
}

