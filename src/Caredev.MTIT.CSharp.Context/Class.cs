using System;
using System.Collections.Generic;
$if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$using Caredev.Mego;
using Caredev.Mego.DataAnnotations;

namespace $rootnamespace$
{
	public class $safeitemrootname$ : DbContext
	{
        public $safeitemrootname$()
            : base($megoargu$)
        { }
$megodbset$
	}
$megoclassdef$
}
