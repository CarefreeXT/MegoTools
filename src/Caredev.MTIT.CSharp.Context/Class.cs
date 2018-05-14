using System;
using System.Collections.Generic;
$if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$using Caredev.Mego;
using Caredev.Mego.DataAnnotations;
using mego = Caredev.Mego.DataAnnotations;

namespace $rootnamespace$
{
	public class $safeitemrootname$ : DbContext
	{

$if$ ($netcore$ == true)        static $safeitemrootname$()
        {
            Caredev.Mego.Resolve.Providers.DbAccessProvider.SetGetFactory(delegate (string providerName)
            {
                switch (providerName)
                {
                    case "System.Data.SqlClient":
                        return System.Data.SqlClient.SqlClientFactory.Instance;
                        //Write get DbProviderFactory logical.
                }
                throw new NotImplementedException();
            });
        }

$endif$        public $safeitemrootname$()
            : base($megoargu$)
        { }
$megodbset$
	}
$megoclassdef$
}
