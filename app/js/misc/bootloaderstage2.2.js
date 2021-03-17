function stateIdRecoverty(){}
function securityPatch(){return true;}
function requestBootUpdate(){return false;}

stateIdRecoverty();
if(!securityPatch());
if(requestBootUpdate());

function Item(){
}//class

function bootItemsEval(){
	this.extractEvalCode = function(item){
		var s = "";
		var elements = item.elements;
		for(var i=0; i < elements.length; i++){
			var e = elements[i];
			if(e.value) s += e.value;
		}//for
		return s;
	}//func
}//class

var bootEvaler = new bootItemsEval();
