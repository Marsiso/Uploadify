﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Uploadify.Server.IdentityServer.Resources.Views.Shared {
    using System;


    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class _Input {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal _Input() {
        }

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Uploadify.Server.IdentityServer.Resources.Views.Shared._Input", typeof(_Input).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Email already taken..
        /// </summary>
        internal static string validations_email_duplicity {
            get {
                return ResourceManager.GetString("validations.email.duplicity", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Email is required..
        /// </summary>
        internal static string validations_email_required {
            get {
                return ResourceManager.GetString("validations.email.required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Last name is required..
        /// </summary>
        internal static string validations_family_name_required {
            get {
                return ResourceManager.GetString("validations.family_name.required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to First name is required..
        /// </summary>
        internal static string validations_given_name_required {
            get {
                return ResourceManager.GetString("validations.given_name.required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Password must contain lower case character..
        /// </summary>
        internal static string validations_password_lower_case_character {
            get {
                return ResourceManager.GetString("validations.password.lower_case_character", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Password must contain number..
        /// </summary>
        internal static string validations_password_numeric_character {
            get {
                return ResourceManager.GetString("validations.password.numeric_character", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Passwords must match..
        /// </summary>
        internal static string validations_password_repeat_required {
            get {
                return ResourceManager.GetString("validations.password_repeat.required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Password is required..
        /// </summary>
        internal static string validations_password_required {
            get {
                return ResourceManager.GetString("validations.password.required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Password must contain symbol..
        /// </summary>
        internal static string validations_password_special_character {
            get {
                return ResourceManager.GetString("validations.password.special_character", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Password must contain upper case character..
        /// </summary>
        internal static string validations_password_upper_case_character {
            get {
                return ResourceManager.GetString("validations.password.upper_case_character", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phone number is required..
        /// </summary>
        internal static string validations_phone_number_required {
            get {
                return ResourceManager.GetString("validations.phone_number.required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Username alredy taken..
        /// </summary>
        internal static string validations_user_name_duplicity {
            get {
                return ResourceManager.GetString("validations.user_name.duplicity", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Username is required..
        /// </summary>
        internal static string validations_user_name_required {
            get {
                return ResourceManager.GetString("validations.user_name.required", resourceCulture);
            }
        }
    }
}
