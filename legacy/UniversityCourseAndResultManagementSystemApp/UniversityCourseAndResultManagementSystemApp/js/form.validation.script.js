$(function(){
	var a={
		submit:{
			settings:{
				form:"#form-signup_v3",
				clear:"keypress",
				inputContainer:".field",
				errorListClass:"ui red pointing below label"
			}
		},
		dynamic:{
			settings:{
				trigger:"keyup",delay:1e3
			},
			callback:{
				onSuccess:function(a,b,c){
					console.log(c+" was pressed!");
					var d=$(b).attr("name");
					return $.inArray(d,["signup_v3[username]","signup_v3[email]"])!==-1&&void 
					
					$.ajax({
						type:"POST",
						data:{form:"signup_v3",input:d,value:$(b).val()},
						url:"/validate/",
						dataType:"json",
						success:function(a,c,d){
							a.status?$(b).parent().find(".ui.corner.label").addClass("green"):($(b).parent().find(".ui.corner.label").removeClass("green"),
							$("#form-signup_v3").addError(a.error))
						}
					})
				},
				onError:function(a,b,c,d){
					$(b).parent().find(".ui.corner.label").removeClass("green")
				}
			}
		}
	};
	
	$.validate(a),
	$("#destroy-signup_v3").click(function(){
		$("#form-signup_v3").validate("destroy")
	}),
	$("#init-signup_v3").click(function(){
		$.validate(a)
	})
});