// <copyright file="IndexRegisterAccountViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using LeeftSamen.Common.InterfaceText;

namespace LeeftSamen.Portal.Web.Models.Register
{
    public class IndexRegisterAccountViewModel : RegisterAccountViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "YourNameIsRequired")]
        [Display(ResourceType = typeof(Label), Name = "RegisterName")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "YourNameIsRequired")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "YourDateOfBirthIsRequired")]
        [Range(0, 32)]
        [Display(ResourceType = typeof(Label), Name = "DateOfBirth")]
        public int DateOfBirthD { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "YourDateOfBirthIsRequired")]
        [Range(0, 13)]
        public int DateOfBirthM { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "YourDateOfBirthIsRequired")]
        [Range(1800, 2400, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "YourDateOfBirthIsRequired")]
        public int DateOfBirthY { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "YourGenderIsRequired")]
        [Display(ResourceType = typeof(Label), Name = "Gender")]
        public string Gender { get; set; }

        //TODO: Opschonen
        public List<SelectListItem> DateOfBirthDays
        {
            get
            {
                var days = new List<SelectListItem> { };

                days.Add(new SelectListItem { Selected = true, Text = Label.Day, Value = "" });
                for (int i = 1; i <= 31; i++)
                {
                    days.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() }); // Kan niet casten?
                }

                return days;
            }
        }

        public List<SelectListItem> DateOfBirthMonths
        {
            get
            {
                var months = new List<SelectListItem> { };

                months.Add(new SelectListItem { Selected = true, Text = Label.Month, Value = "" });
                for (int i = 1; i <= 12; i++)
                {
                    months.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }

                return months;
            }
        }

        public List<SelectListItem> DateOfBirthYears
        {
            get
            {
                var years = new List<SelectListItem> { };

                years.Add(new SelectListItem { Selected = true, Text = Label.Year, Value = "" });
                for (int i = 0; i <= 105; i++)
                {
                    years.Add(new SelectListItem { Text = DateTime.Today.AddYears(-i).Year.ToString(), Value = DateTime.Today.AddYears(-i).Year.ToString() });
                }

                return years;
            }
        }
    }
}

