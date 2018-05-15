var json;
var selectedAnswer;
//Set lowest score item index
var nextItemIndex = 0;

var SelectedAnswerID;
var CorrectAnswerID;


function Start(jsonParameter)
{
    json = jsonParameter;
    document.getElementById("Word").innerHTML = json.items[0].word;
    RandomlySetAnswers(json.items[0].meaning, json.items[0].option1, json.items[0].option2, json.items[0].option3);

    document.getElementById("btnStart").style.display = "none";
    document.getElementById("btnNext").style.display = "block";
    document.getElementById("table").style.display = "block";
    document.getElementById("btnFinish").style.display = "none";
}



function Next()
{
    if (selectedAnswer === json.items[nextItemIndex].meaning)
    {
        json.items[nextItemIndex].score = 1;
        Reset();
        document.getElementById(CorrectAnswerID).style.backgroundColor = "green";
    }
    else
    {
        json.items[nextItemIndex].score = 0;
        Reset();
        document.getElementById(SelectedAnswerID).style.backgroundColor = "red";
        document.getElementById(CorrectAnswerID).style.backgroundColor = "green";
    }

    window.setTimeout(function () {
        Reset();
        if (CheckIfAllItemsFinished() === true) {
            alert("Test Over");

            document.getElementById("btnNext").style.display = "none";
            document.getElementById("table").style.display = "none";
            document.getElementById("btnFinish").style.display = "block";
        }
        else {
            //Get the first item with lowest score
            for (var i = 0; i < json.items.length; i++) {
                if (i !== nextItemIndex && json.items[i].score===0)
                {
                    nextItemIndex = i;
                    break;
                }
            }

            document.getElementById("Word").innerHTML = json.items[nextItemIndex].word;
            RandomlySetAnswers(json.items[nextItemIndex].meaning, json.items[nextItemIndex].option1, json.items[nextItemIndex].option2, json.items[nextItemIndex].option3);
        }
    }, 1000);
    
}

function Select(selectedID, otherID1, otherID2, otherID3)
{
    document.getElementById(selectedID).style.backgroundColor = "blue";
    document.getElementById(otherID1).style.backgroundColor = "white";
    document.getElementById(otherID2).style.backgroundColor = "white";
    document.getElementById(otherID3).style.backgroundColor = "white";

    selectedAnswer = document.getElementById(selectedID).innerHTML;
    SelectedAnswerID = selectedID;
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
    var answerIDPair = {};
    document.getElementById("Meaning").innerHTML = answerArray[answerIndex];
    answerIDPair[answerArray[answerIndex]] ="Meaning";
    answerArray.splice(answerIndex, 1);

    answerIndex = GetRandomIndex(answerArray);
    document.getElementById("Ans1").innerHTML = answerArray[answerIndex];
    answerIDPair[answerArray[answerIndex]] = "Ans1";
    answerArray.splice(answerIndex, 1);


    answerIndex = GetRandomIndex(answerArray);
    document.getElementById("Ans2").innerHTML = answerArray[answerIndex];
    answerArray.splice(answerIndex, 1);
    answerIDPair[answerArray[answerIndex]] = "Ans2";

    answerIndex = GetRandomIndex(answerArray);
    document.getElementById("Ans3").innerHTML = answerArray[answerIndex];
    answerIDPair[answerArray[answerIndex]] = "Ans3";
    CorrectAnswerID = answerIDPair[answer1];
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
        if (json.items[i].score === 1)
        {
            result = true;
        }
        else if (json.items[i].score ===0)
        {
            result = false;
            break;
        }
    }
    return result;
}