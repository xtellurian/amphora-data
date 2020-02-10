var attributes = {}
var containerElementId = {};
function attributeEditor(initialAttributes, containerElementIdInput, inputElementIdInput) {
    // attributes is a Dictionary<string, CustomKVP>
    attributes = initialAttributes;
    containerElementId = containerElementIdInput;
    inputElementId = inputElementIdInput;
    redrawUI()
    updateMetadataInput();
}

function updateMetadataInput() {
    var metadataInput = document.getElementById(inputElementId);
    metadataInput.value = JSON.stringify(attributes);
}

function redrawUI() {
    console.log("Updating UI")
    // just clear and redo every time
    var html = "";
    for (var index of Object.keys(attributes)) {
        html += `
        <div id="metaContainerChild${index}" class="row form-group">
            <div class="col-5">
                <input class="form-control" type=text id="key${index}" value="${attributes[index].Key}" onchange="keyChanged(${index})">
            </div>
            <div class="col-5">
                <input class="form-control" type=text id="value${index}" value="${attributes[index].Value}" onchange="valueChanged(${index})">
            </div>
            <div class="col-2">
                <button class="btn btn-danger" onClick=removeMetaRow(${index})>Delete</button>
            </div>
        </div>
        `
    }
    document.getElementById(containerElementId).innerHTML = html;
}

function addMetaDataRow(newAttribute) {
    // first check if ther bottom key is empty
    var maxIndex = Math.max(...Object.keys(attributes), 0);
    if(attributes[maxIndex] && !attributes[maxIndex].Key) return; // return if bottom key is empty

    var index = Math.max(...Object.keys(attributes), 0);
    if(Object.keys(attributes).length > 0) {
        // not the first entry
        index += 1;
    }

    if(! newAttribute) {
        newAttribute = {Key: "", Value: ""}
    } 
    attributes[index] = newAttribute;
    redrawUI();
    updateMetadataInput();
}

function keyChanged(index) {
    var newKey = document.getElementById(`key${index}`).value
    attributes[index].Key = newKey;
    updateMetadataInput();
}

function valueChanged(index) {
    var newValue = document.getElementById(`value${index}`).value
    attributes[index].Value = newValue;
    updateMetadataInput();
}

function removeMetaRow(index) {
    delete attributes[index]
    updateMetadataInput()
    redrawUI()
}