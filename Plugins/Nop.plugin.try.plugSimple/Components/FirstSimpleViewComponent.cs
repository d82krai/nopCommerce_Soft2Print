﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.plugin.FirstTry.PlugSimple.Components
{
    public  class FirstSimpleViewComponent: NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/FirstTry.PlugSimple/Views/FirstSimpleView.cshtml");
        }

    }
}
