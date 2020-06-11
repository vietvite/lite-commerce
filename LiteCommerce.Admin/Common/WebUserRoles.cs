using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiteCommerce
{
    /// <summary>
    /// Định nghĩa danh sách các Role của user
    /// </summary>
    public class WebUserRoles
    {
        /// <summary>
        /// Không xác định
        /// </summary>
        public const string ANONYMOUS = "anonymous";
        /// <summary>
        /// Staff
        /// </summary>
        public const string STAFF = "staff";
        /// <summary>
        /// System management.
        /// </summary>
        public const string ADMINISTRATOR = "administrator";
        /// <summary>
        /// Human resource manager. Respond for manage accounts
        /// </summary>
        public const string HR_MANAGER = "hr_manager";
        /// <summary>
        /// Sale man. Respond for manage orders
        /// </summary>
        public const string SALEMAN = "saleman";
        /// <summary>
        /// Data management staff. Respond for manage catalogs
        /// </summary>
        public const string DATA_MANAGER = "data_manager";
    }
}