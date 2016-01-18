function MudarTipoPessoa(tipo)    
{
    if (tipo==1)
    {
        alert('1');
        document.getElementById("lbTipo").innerHTML= "BLA";        
    } 
    
    if (tipo==2)
    {
        alert('2');        
        document.getElementById("lbTipo").innerHTML= "BOOM";
    } 
}

function aumenta(obj){
    obj.height=obj.height*2;
	obj.width=obj.width*2;
}

function diminui(obj){
	obj.height=obj.height/2;
	obj.width=obj.width/2;
}

function showHide (ID) {
	if (document.getElementById(ID).style.display == "none") {
	   document.getElementById(ID).style.display= "";
	   document.getElementById(ID + "_span").innerHTML= "[-]";
	}
	else {
	   document.getElementById(ID).style.display = "none";
	   document.getElementById(ID + "_span").innerHTML= "[+]";
	}
}


function showHideAcesso (ID) {
	if (document.getElementById(ID).style.display == "none") {
	   document.getElementById(ID).style.display= "";
	   document.getElementById(ID + "_span").innerHTML= "<<";
	}
	else {
	   document.getElementById(ID).style.display = "none";
	   document.getElementById(ID + "_span").innerHTML= ">>";
	}
}


function ConfirmaExclusaoItem (id_registro) {

  if (confirm("Confirma a exclusao do registro ?"))
  {
	 document.pesquisar.id_exc.value = id_registro;
	 document.pesquisar.submit();
  }

}


//--->Função para a formatação dos campos...<---
function Mascara(tipo, campo, teclaPress) {
        if (window.event)
        {
                var tecla = teclaPress.keyCode;
        } else {
                tecla = teclaPress.which;
        }
 
        var s = new String(campo.value);
        // Remove todos os caracteres à seguir: ( ) / - . e espaço, para tratar a string denovo.
        s = s.replace(/(\.|\(|\)|\/|\-| )+/g,'');
 
        tam = s.length + 1;
        
        if ( tecla != 9 && tecla != 8 ) {
                switch (tipo)
                {
                case 'CPF' :
                        if (tam > 3 && tam < 7)
                                campo.value = s.substr(0,3) + '.' + s.substr(3, tam);
                        if (tam >= 7 && tam < 10)
                                campo.value = s.substr(0,3) + '.' + s.substr(3,3) + '.' + s.substr(6,tam-6);
                        if (tam >= 10 && tam < 12)
                                campo.value = s.substr(0,3) + '.' + s.substr(3,3) + '.' + s.substr(6,3) + '-' + s.substr(9,tam-9);
                break;
 
                case 'CNPJ' :
 
                        if (tam > 2 && tam < 6)
                                campo.value = s.substr(0,2) + '.' + s.substr(2, tam);
                        if (tam >= 6 && tam < 9)
                                campo.value = s.substr(0,2) + '.' + s.substr(2,3) + '.' + s.substr(5,tam-5);
                        if (tam >= 9 && tam < 13)
                                campo.value = s.substr(0,2) + '.' + s.substr(2,3) + '.' + s.substr(5,3) + '/' + s.substr(8,tam-8);
                        if (tam >= 13 && tam < 15)
                                campo.value = s.substr(0,2) + '.' + s.substr(2,3) + '.' + s.substr(5,3) + '/' + s.substr(8,4)+ '-' + s.substr(12,tam-12);
                break;
 
                case 'TEL' :
                        if (tam > 2 && tam < 4)
                                campo.value = '(' + s.substr(0,2) + ') ' + s.substr(2,tam);
                        if (tam >= 7 && tam < 11)
                                campo.value = '(' + s.substr(0,2) + ') ' + s.substr(2,4) + '-' + s.substr(6,tam-6);
                break;
 
                case 'DATA' :
                        if (tam > 2 && tam < 4)
                                campo.value = s.substr(0,2) + '/' + s.substr(2, tam);
                        if (tam > 4 && tam < 11)
                                campo.value = s.substr(0,2) + '/' + s.substr(2,2) + '/' + s.substr(4,tam-4);
                break;
                
                case 'CEP' :
                        if (tam > 5 && tam < 7)
                                campo.value = s.substr(0,5) + '-' + s.substr(5, tam);
                break;
                }
        }
}


function ConfirmaExclusao (link_php) {
	
  var registros = ItensSelec();
  
  if (registros=="") {
	  alert('Selecione pelo menos um registro para exclusão');
	  return false;	  
  }
  
  if (confirm("Confirma a exclusão do(s) registro(s) ?"))
  {       
	 document.pesquisar.id_exc.value = registros;
	 document.pesquisar.submit();
  }  
  
}

function ShowDv(div, close_auto)
{
	document.getElementById(div).style.display = 'block';
	
	if(close_auto == 1)
	{
		var x = setTimeout("CloseDv('"+ div +"')", 10000);
	}
}

function CloseDv(div)
{
	document.getElementById(div).style.display = 'none';
}

function ShowMod(div, opt, grp)
{
	for(i=1;i<=grp;i++)
	{
		//alert(i);
		if(i==opt)
		{
			//alert('mostre' + i);
			ShowDv(div + '' + i, 0);
		}else
		{
			//alert('esconda' + i);
			CloseDv(div + '' + i);
		}
	}
}


function SetEmpresa(varEmpresa, varServer, varLocation)
{

	document.location.href = "../change.php?e="+ varEmpresa + "&l=http://"+ varServer + "" + varLocation;

}


function FloatValidate(objeto, aceitaVirgula, e)
{
	var key;
	var keychar;
	var keydecimal;
	var decimal=0;
	
	if(!e.keyCode)
	{
		key = e.which;
	}
	else
	{
		key = e.keyCode;
	}
	//}
	
	//alert(key);
	keychar = String.fromCharCode(key).toLowerCase();

	if(key==8 || key==9 || key==13 || key==35 || key==36 || key==37 || key==39 || key==46 || key==116)
	{
		if(!e.keyCode && keychar==".")
		{
			return false;
		}
		else
			return true;
	}
	
	for(var i=0;i<objeto.length;i++)
	{
		//alert(objeto.substr(i,1));
		if(objeto.substr(i,1)==",")
		{
			decimal = i;
		}
	}
	if(("0123456789").indexOf(keychar) > -1)
	{
		if(decimal == 0)
		{
			return true;
		}
		else
		{
			if(decimal+2>=objeto.length)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
	else if((keychar==",") && (aceitaVirgula==true))
	{
		return (decimal == 0)
	}
	else
	{
		return false;
	}
}

function pulacampo(valor, qtde_caracteres, formulario, proximocampo)
{
	if(valor.length==qtde_caracteres)
	{
		eval("document."+formulario+"."+proximocampo+".focus()");
	}
}


function EditItem(f_action, tp, id)
{
	document.foo.action = f_action;
	document.foo.id.value = id;
	document.foo.tp.value = tp;
	
	document.foo.submit();
}


function ChGuia(opt, limit)
{

	for(i=1;i<=limit;i++){
		if(i==opt){
			document.getElementById('guia' + i).className='guia_sel';
			ShowDv('aba' + i, 0);
		}else{
			document.getElementById('guia' + i).className='guia';
			CloseDv('aba' + i);
		}
	}

	document.location = '#';
	
}


function SelecionarTodos(valor)
{
	var obj = document.resultado.id_multiple;
	
	for(i=0;i<(obj.length-1);i++)
	{
		obj[i].checked = valor;
	}
	
}

function QtdItensSelec()
{
	var itens = document.resultado.id_multiple;
	var count = 0;
	var selec = "";
	
	for(i=0;i<itens.length;i++)
	{
		if(itens[i].checked == true)
		{
			count++;
			if(selec == ""){
				selec = itens[i].value;
			}else{
				selec = selec + ", " + itens[i].value;
			}
		}
	}

	document.foo.id.value = selec;
	return count;
	
}

function SetPage(varPage, varServer, varLocation)
{

	document.location.href = "http://"+ varServer + "" + varLocation +"?pag=" + varPage;

}

function PopUp(url, name, width, height)
{
	
	var str = "height=" + height;
	str += ",width=" + width;
    str += ",status=yes,scrollbars=yes,resizable=no";

	if (window.screen)
	{
		var ah = screen.availHeight - 30;
		var aw = screen.availWidth - 10;
		var xc = (aw - width) / 2;
		var yc = (ah - height) / 2;

		str += ",left=" + xc + ",screenX=" + xc;
		str += ",top=" + yc + ",screenY=" + yc;
	}
				
	var win = window.open(url, name, str);
	win.focus();
	
}


function Destrava(linha_atual, proximo){

	try{
	
		a = document.incluir.tele_tipo;
		b = document.incluir.tele_ddd;
		c = document.incluir.tele_fone;
		d = document.incluir.tele_ramal;
		
		for(i=0;i<4;i++)
		{
			if(i==linha_atual)
			{

				if(a[i].value!="" && b[i].value!="" && c[i].value!="" && linha_atual!=3)
				{
					a[proximo].disabled = false;
					b[proximo].disabled = false;
					c[proximo].disabled = false;
					d[proximo].disabled = false;
				}else{
					
					if(linha_atual!=3)
					{
						a[proximo].disabled = true;
						b[proximo].disabled = true;
						c[proximo].disabled = true;
						d[proximo].disabled = true;
						
						a[proximo].value = "";
						b[proximo].value = "";
						c[proximo].value = "";
						d[proximo].value = "";

					}
					
				}
			}
		}
		
	}catch(e){ alert(e); }
	
}


function is_date(date)
{
	if(date != "")
	{
		if(date.length != 10)
		{
			return false; //DATA INVALIDA
		}
		var err = 0;
		var dia = date.substring(0,2);
		var mes = date.substring(3,5);
		var ano = date.substring(6,10);
		
		if(isNumber(dia) && dia > 31)
		{
			err = 1; //DIA INVALIDO
			return(false);
		}
		if( (mes < 1 && mes > 12)  && isNumber(mes))
		{
			err = 2; //MES INVALIDO
			return(false);
		}
		if( (ano.length != 4) && isNumber(ano) )
		{
			err = 3; //ANO INVALIDO
			return(false);
		}
		
		//VALIDAÇÃO DE ANO BISSEXTO
		if(mes == 2)
		{
			if ( ( dia > 28 ) && ( ( ano % 4 ) == 0 ) )
				return false; // ANO NÃO É BISSEXTO, DATA INVALIDA
			//Valido ano bissexto
			if ( ( dia > 29 ) && ( ( ano % 4 ) != 0 ) )
				return false; //ANO BISSEXTO, DATA INVALIDA
		}
		
		return true;
	}
	else
	{
		return true;
	}
}


function validaCNPJ(CNPJ) {
	erro = new String;
	if (CNPJ.length < 14) 
		erro += "É necessario preencher corretamente o número do CNPJ! \n\n";
	if ((CNPJ.charAt(2) == ".") || (CNPJ.charAt(6) == ".") || (CNPJ.charAt(10) == "/") || (CNPJ.charAt(15) == "-")){
		if(document.layers && parseInt(navigator.appVersion) == 4){
			x = CNPJ.substring(0,2);
			x += CNPJ. substring (3,6);
			x += CNPJ. substring (7,10);
			x += CNPJ. substring (11,15);
			x += CNPJ. substring (16,18);
			CNPJ = x;
		} 
		else {
			CNPJ = CNPJ. replace (".","");
			CNPJ = CNPJ. replace (".","");
			CNPJ = CNPJ. replace ("-","");
			CNPJ = CNPJ. replace ("/","");
		}
	}
	var nonNumbers = /\D/;
	if (nonNumbers.test(CNPJ)) 
		erro += "A verificação de CNPJ suporta apenas números! \n\n";
	var a = [];
	var b = new Number;
	var c = [6,5,4,3,2,9,8,7,6,5,4,3,2];
	for (i=0; i<12; i++){
		a[i] = CNPJ.charAt(i);
		b += a[i] * c[i+1];
	}
	if ((x = b % 11) < 2){
		a[12] = 0 
	} else { 
		a[12] = 11-x 
	}
	b = 0;
	for (y=0; y<13; y++) {
		b += (a[y] * c[y]);
	}
	if ((x = b % 11) < 2) { 
		a[13] = 0; 
	} else { 
		a[13] = 11-x; 
	}
	if ((CNPJ.charAt(12) != a[12]) || (CNPJ.charAt(13) != a[13])){
		erro +="Dígito verificador com problema!";
	}
	if (erro.length > 0){
		return false;
	} else {
		return true;
	}
}


/*
Nome da Função: ValidaEmail(email)
  Parametro email: string que contem o email a ser validado
  Retorno: booleano
Descrição da Função: funcao para verificar se o email digitado esta correto
Desenvolvido por: Joao Paulo Siqueira
Data: 08/06/2008
*/
function ValidaEmail(email) {
	try {
		er = /^[0-9a-z][0-9a-zA-Z._-]+@[a-z0-9][-.a-z0-9]+[.][a-z]+$/;
		if (!er.test(email)) {
			return false;
		}
		return true;
	} catch (e) {}
}


/*
Nome da Função: DivulgarPortalMulti(valor, frm)
  Parametro valor: valor do campo checkbox
  Parametro frm: objeto frm
Descrição da Função: habilitar/desabilitar linhaformulario para divulgação no portal
Desenvolvido por: Joao Paulo Siqueira
Data: 26/12/2008
*/
function DivulgarPortalMulti(valor, frm, box){

	frm.departamento_portal.disabled = !valor;
	frm.categoria_portal2.disabled = !valor;
	frm.categoria_portal3.disabled = !valor;
	frm.categoria_portal4.disabled = !valor;
	frm.categoria_portal5.disabled = !valor;

	if(valor==true){
		frm.departamento_portal.value = frm.departamento.value;
		fu_carrega_secao_p_multi(frm.departamento_portal.value, 2, '', 't', box);
		fu_carrega_secao_p_multi(0, 3, '', 't', box);
		fu_carrega_secao_p_multi(0, 4, '', 't', box);
		fu_carrega_secao_p_multi(0, 5, '', 'f', box);
	}
	
}


/*
Nome da Função: OpenMsgs(param1)
  Parametro param1: parametro de mensagens a buscar
Descrição da Função: Abre Mensagens Padrão
Desenvolvido por: Joao Paulo Siqueira
Data: 23/01/2009
*/
function OpenMsgs(param1){
	
	PopUp('../pop/GetMensagensPadrao.php?param='+param1, 'gmp', 500, 800);
	
}



/*
Nome da Função: ValidaCPF(valor)
  Parametro valor: contem o valor que vai ser validado
  Retorno: booleano
Descrição da Função: funcao para validar se o CPF e valido
Desenvolvido por: Joao Paulo Wodiani
Data: 08/06/2008
 */
function ValidaCPF(valor){
	try {

		s = valor;
		if (isNaN(s)) {
			return false;
		}
		
		var i;
		var c = s.substr(0,9);
		var dv = s.substr(9,2);
		var d1 = 0;
		for (i = 0; i < 9; i++) {
			d1 += c.charAt(i)*(10-i);
		}
		if (d1 == 0){
			return false;
		}
	    d1 = 11 - (d1 % 11);
	    if (d1 > 9) d1 = 0;         
		if (dv.charAt(0) != d1) {
			return false;         
		}
		d1 *= 2;
		for (i = 0; i < 9; i++) {
			d1 += c.charAt(i)*(11-i);
		}
		d1 = 11 - (d1 % 11);
		if (d1 > 9) d1 = 0;
		if (dv.charAt(1) != d1) {
			return false;
		}
	    return true;
	} catch (e) {}
} 


function ItensSelec()
{
	var itens = document.pesquisar.id_multiple;
	var selec = "";
	var cont=0;
	
	for(i=0;i<itens.length;i++)
	{
		if(itens[i].checked == true)
		{
			cont++;
			if(selec == ""){
				selec = itens[i].value;
			}else{
				selec = selec + ", " + itens[i].value;
			}
		}
	}
	
	//SE TIVER APENAS UM ITEM
	if (cont==0 && itens.value!="") {
		if (itens.checked) {
			selec = itens.value;	
		}
	}
	return selec;
	
}


/*
Nome da Função: ValidaData(data)
  Parametro data: contem a data a ser validada
Descrição da Função: funcao para validar se a data e valida
Desenvolvido por: Joao Paulo W Siqueira
Data: 08/06/2008
 */
function ValidaData (data) { 
	try {
		dia = (data.substring(0,2)); 
		mes = (data.substring(3,5)); 
		ano = (data.substring(6,10)); 
	
		situacao = true; 
		// verifica o dia valido para cada mes 
		if (isNaN(dia) || (dia < 01)||(dia < 01 || dia > 30) && (  mes == 04 || mes == 06 || mes == 9 || mes == 11 ) || dia > 31) { 
			situacao = false; 
		}
		// verifica se o mes e valido 
		if (isNaN(mes) || mes < 01 || mes > 12 ) { 
			situacao = false; 
		}
		if (isNaN(ano) ) { 
			situacao = false; 
		}	
		// verifica se e ano bissexto 
		if (mes == 2 && ( dia < 01 || dia > 29 || ( dia > 28 && (parseInt(ano / 4) != ano / 4) ))) { 
			situacao = false; 
		} 
		if (data.value == "") { 
			situacao = false; 
		} 
		if (dia.toLowerCase() == "xx" && mes.toLowerCase() == "xx" && ano.toLowerCase() == "xxxx") {
			situacao = true;
		}
		return situacao;
	} catch (e) {}
}