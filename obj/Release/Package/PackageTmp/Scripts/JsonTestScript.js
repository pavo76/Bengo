var json;
var selectedAnswer;
//Set lowest score item index
var nextItemIndex=0;


function Start(jsonParameter)
{
    json = jsonParameter;
    document.getElementById("Word").innerHTML = json.items[0].word;
    RandomlySetAnswers(json.items[0].meaning, json.items[0].option1, json.items[0].option2, json.items[0].option3);
    document.getElementById("Score").innerHTML = json.items[0].score + "/4";

    document.getElementById("btnStart").style.display = "none";
    document.getElementById("btnNext").style.display = "block";
    document.getElementById("table").style.display = "block";
    document.getElementById("btnFinish").style.display = "none";
}



function Next()
{
    if (selectedAnswer == json.items[nextItemIndex].meaning)
    {
        json.items[nextItemIndex].score += 1;
    }
    else
    {
        if (json.items[nextItemIndex].score >= 1)
        {
            json.items[nextItemIndex].score -= 1;
        }
    }

    window.setTimeout(function () {
        Reset();
        if (CheckIfAllItemsFinished() == true) {
            alert("Test Over");

            document.getElementById("btnNext").style.display = "none";
            document.getElementById("table").style.display = "none";
            document.getElementById("btnFinish").style.display = "block";
        }
        else {
            //Get the first item with lowest score
            for (var i = 0; i < json.items.length; i++) {
                if (i != nextItemIndex) {
                    if (i == 0) {
                        nextItemIndex = i;
                    }
                    else {
                        if (json.items[nextItemIndex].score >= json.items[i].score) {
                            nextItemIndex = i;
                        }
                    }
                }
            }

            document.getElementById("Word").innerHTML = json.items[nextItemIndex].word;
            RandomlySetAnswers(json.items[nextItemIndex].meaning, json.items[nextItemIndex].option1, json.items[nextItemIndex].option2, json.items[nextItemIndex].option3);
            document.getElementById("Score").innerHTML = json.items[nextItemIndex].score + "/4";
        }
    }, 1000);
    
}

function Select(selectedID, otherID1, otherID2, otherID3)
{
    document.getElementById(selectedID).style.backgroundColor = "green";
    document.getElementById(otherID1).style.backgroundColor = "white";
    document.getElementById(otherID2).style.backgroundColor = "white";
    document.getElementById(otherID3).style.backgroundColor = "white";

    selectedAnswer = document.getElementById(selectedID).innerHTML;
}

function Reset()
{
    document.getElementById("Meaning").style.backgroundColor = "white";
    document.getElementById("Ans1").style.backgroundColor = "white";
    document.getElementById("Ans2").style.backgroundColor = "white";
    document.getElementById("Ans3").style.backgroundColor = "white";
    selectedAnswer = "";
}

function RandomlySetAnswers(answer1, answer2, answer3, answer4)
{
    var answerArray = Array(answer1, answer2, answer3, answer4);
    var answerIndex = GetRandomIndex(answerArray);
    document.getElementById("Meaning").innerHTML = answerArray[answerIndex];
    answerArray.splice(answerIndex, 1);
    answerIndex = GetRandomIndex(answerArray);
    document.getElementById("Ans1").innerHTML = answerArray[answerIndex];
    answerArray.splice(answerIndex, 1);
    answerIndex = GetRandomIndex(answerArray);
    document.getElementById("Ans2").innerHTML = answerArray[answerIndex];
    answerArray.splice(answerIndex, 1);
    answerIndex = GetRandomIndex(answerArray);
    document.getElementById("Ans3").innerHTML = answerArray[answerIndex];
}

function GetRandomIndex(array)
{
    var index = Math.floor((Math.random() * array.length));
    return index;
}


function CheckIfAllItemsFinished()
{
    var result = false;
    for (var i = 0; i < json.items.length; i++)
    {
        if (json.items[i].score == 4)
        {
            result = true;
        }
        else if (json.items[i].score < 4)
        {
            result = false;
            break;
        }
    }
    return result;
}