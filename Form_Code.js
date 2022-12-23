

function createNewAccount(executionContext) {
debugger;
var formContext = executionContext.getFormContext();

var accountID = formContext.getAttribute("name").getValue();
alert(accountID);

var id = formContext.getAttribute("accountid").getValue();

/*let primaryContact = primaryContact.replace("{","");
let primaryContact = primaryContact.replace("}","");
*/

console.log("Name is: " + accountID);
console.log(id);


}

function getContactId(executionContext){
    debugger;
    var formContext = executionContext.getFormContext();

    var contactId = formContext.data.entity.getId();
    contactId = contactId.replace("{", "").replace("}", "");
    
    console.log(contactId);
}

function getSubgrid(executionContext){
    debugger;
    var formContext = executionContext.getFormContext();
    var gridContect = formContext.getControl("QuestionsDetail");
    var gridRows = gridContect.getGrid().getRows();

    console.log(gridRows);
}                                                                                                           