using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

public class BaseWebUi : System.Web.UI.Page
{
	protected override void OnInit(EventArgs e)
	{
		//se o div de Aguarde ainda estiver mostrando ele tira
		ScriptManager src = ScriptManager.GetCurrent(Page);
		if (src != null)
			ScriptManager.RegisterClientScriptBlock(
				this,
				typeof(void),
				"TiraDivAguarde",
				"if(document.getElementById('divProcessando')) document.getElementById('divProcessando').style.display = 'none';",
				true);
		else
			ClientScript.RegisterStartupScript(
				typeof(Page),
				"TiraDivAguarde",
				"if(document.getElementById('divProcessando')) document.getElementById('divProcessando').style.display = 'none';",
				true);

		ClientScript.RegisterOnSubmitStatement(
			this.GetType(),
			"zerarfiltro",
			"if(document.getElementById('divProcessando') && document.getElementById('divProcessando').style.display!='none')return false;");

		ClientScript.RegisterOnSubmitStatement(
			this.GetType(),
			"Aguarde",
			"if (typeof(ValidatorOnSubmit) == 'function' && ValidatorOnSubmit() == false) return false; avisoAguarde();");
			
		base.OnInit(e);
	}
}

