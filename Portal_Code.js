
$(document).ready(function ()
{
	
    $("#cr0e4_position").change(showField);
	showField();
    hideSection();
    showSection();
    
	
});

function showField()
{
	var position = $("#cr0e4_position").find("option:selected").text();
	alert(position)
}

function hideSection() {
	//$("table[data-name='questions']").parent().hide();
    $(".section[data-name='questions']").closet("fieldset").hide();
}

function showSection() {
	var position = $("#cr0e4_position").find("option:selected").text();
if (position == "Admin") {
    $(".section[data-name='questions']").closet("fieldset").show();
    $("table[data-name='questions']").parent().parent().show();
    alert("Function_kickingoff");
}
else {
    alert("functionNoFire")
};
}

