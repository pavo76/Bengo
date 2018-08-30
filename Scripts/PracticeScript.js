var json;
var selectedAnswer;
//Set lowest score item index
var nextItemIndex = 0;


function Start(jsonParameter)
{
    json = jsonParameter;
    document.getElementById("Word").innerHTML = json.items[0].word;
    RandomlySetAnswers(json.items[0].meaning, json.items[0].option1, json.items[0].option2, json.items[0].option3);
    document.getElementById("Score").innerHTML = json.items[0].finished + "/" + json.items.length;

    document.getElementById("btnStart").style.display = "none";
    //document.getElementById("btnNext").style.display = "block";
    document.getElementById("test").style.display = "block";
    document.getElementById("btnFinish").style.display = "none";
}



function Next()
{
    answerIndicator = document.getElementById("answerIndicator");
    if (selectedAnswer === json.items[nextItemIndex].meaning)
    {
        if (json.items[nextItemIndex].score == 0) {
            json.items[nextItemIndex].score = 1;
        }
        json.items[nextItemIndex].finished = 1;
        answerIndicator.style.backgroundColor = "yellowgreen";
        answerIndicator.innerHTML = "Correct!"
        answerIndicator.style.display = "block";
    }
    else
    {
        if (json.items[nextItemIndex].score == 0)
        {
            json.items[nextItemIndex].score = -1;
        }
        else if(json.items[nextItemIndex].score == -1)
        {
            json.items[nextItemIndex].score = -2;
        }
        

        answerIndicator.style.backgroundColor = "indianred";
        answerIndicator.innerHTML = "Wrong! The correct answer is: " + json.items[nextItemIndex].meaning;
        answerIndicator.style.display = "block";
    }

    window.setTimeout(function () {
        Reset();

        answerIndicator.style.display = "none";
        if (CheckIfAllItemsFinished() === true) {
            document.getElementById("test").style.display = "none";
            document.getElementById("btnFinish").style.display = "block";
            var link = document.getElementById("FinishLink").href;

            var goodIDstring = "";
            for (var i = 0; i < json.items.length; i++)
            {
                if (json.items[i].score == 1)
                {
                    if (goodIDstring == "") {
                        goodIDstring += json.items[i].id;
                    }
                    else
                    {
                        goodIDstring +=","+ json.items[i].id;
                    }
                }
            }
            var okayIDstring = "";
            for (var i = 0; i < json.items.length; i++) {
                if (json.items[i].score == -1) {
                    if (okayIDstring == "") {
                        okayIDstring += json.items[i].id;
                    }
                    else {
                        okayIDstring += "," + json.items[i].id;
                    }
                }
            }
            var badIDstring = "";
            for (var i = 0; i < json.items.length; i++) {
                if (json.items[i].score == -2) {
                    if (badIDstring == "") {
                        badIDstring += json.items[i].id;
                    }
                    else {
                        badIDstring += "," + json.items[i].id;
                    }
                }
            }
            if (goodIDstring != "")
            {
                link = link.replace("_Good", goodIDstring);
            }
            if (okayIDstring != "") {
                link = link.replace("_Okay", okayIDstring);
            }
            if (badIDstring != "") {
                link = link.replace("_Bad", badIDstring);
            }
            document.getElementById("FinishLink").href = link;
        }
        else {
            //Get the first non finished item or stay on same if last unfinished
            for (var i = 0; i < json.items.length; i++) {
                if (i !== nextItemIndex && json.items[i].finished===0)
                {
                    nextItemIndex = i;
                    break;
                }
            }

            document.getElementById("Word").innerHTML = json.items[nextItemIndex].word;
            RandomlySetAnswers(json.items[nextItemIndex].meaning, json.items[nextItemIndex].option1, json.items[nextItemIndex].option2, json.items[nextItemIndex].option3);
            var score = 0;
            for (var i = 0; i < json.items.length; i++) {
                score += json.items[i].finished;
            }
            document.getElementById("Score").innerHTML = score + "/" + json.items.length;
        }
    }, 2000);
    
}

function Select(selectedID, otherID1, otherID2, otherID3)
{
    selectedAnswer = document.getElementById(selectedID).value;
}

function Reset()
{
    selectedAnswer = "";
}

function RandomlySetAnswers(answer1, answer2, answer3, answer4)
{
    var answerArray = Array(answer1, answer2, answer3, answer4);
    var answerIndex = GetRandomIndex(answerArray);
    document.getElementById("Meaning").value = answerArray[answerIndex];
    answerArray.splice(answerIndex, 1);
    answerIndex = GetRandomIndex(answerArray);
    document.getElementById("Ans1").value = answerArray[answerIndex];
    answerArray.splice(answerIndex, 1);
    answerIndex = GetRandomIndex(answerArray);
    document.getElementById("Ans2").value = answerArray[answerIndex];
    answerArray.splice(answerIndex, 1);
    answerIndex = GetRandomIndex(answerArray);
    document.getElementById("Ans3").value = answerArray[answerIndex];   
}

function GetRandomIndex(array)
{
    var index = Math.floor(Math.random() * array.length);
    return index;
}


function CheckIfAllItemsFinished()
{
    var result = false;
    for (var i = 0; i < json.items.length; i++)
    {
        if (json.items[i].finished === 1)
        {
            result = true;
        }
        else if (json.items[i].finished ===0)
        {
            result = false;
            break;
        }
    }
    return result;
}