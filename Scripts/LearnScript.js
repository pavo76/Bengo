var json;
var selectedAnswer;
//Set lowest score item index
var nextItemIndex=0;


function Start(jsonParameter)
{
    json = jsonParameter;
    document.getElementById("Word").innerHTML = json.items[0].word;
    RandomlySetAnswers(json.items[0].meaning, json.items[0].option1, json.items[0].option2, json.items[0].option3);
    document.getElementById("Score").innerHTML = json.items[0].score + "/" + json.items.length*4;

    document.getElementById("btnStart").style.display = "none";
    //document.getElementById("btnNext").style.display = "block";
    document.getElementById("test").style.display = "block";
    document.getElementById("btnFinish").style.display = "none";
    document.getElementById("Explanation").style.display = "none";
}



function Next()
{
    answerIndicator = document.getElementById("answerIndicator");
    if (selectedAnswer === json.items[nextItemIndex].meaning)
    {
        json.items[nextItemIndex].score += 1;
        answerIndicator.style.backgroundColor = "yellowgreen";
        answerIndicator.innerHTML = "Correct!"
        answerIndicator.style.display = "block";

    }
    else
    {
        if (json.items[nextItemIndex].score >= 1)
        {
            json.items[nextItemIndex].score -= 1;
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
        }
        else {
            //Get the first item with lowest score
            for (var i = 0; i < json.items.length; i++) {
                if (i !== nextItemIndex) {
                    if (i === 0) {
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
            var score = 0;
            for (var i = 0; i < json.items.length; i++)
            {
                score += json.items[i].score;
            }
            document.getElementById("Score").innerHTML = score + "/" + json.items.length*4;
        }
    }, 1500);
    
}

function Select(selectedID, otherID1, otherID2, otherID3)
{
    //document.getElementById(selectedID).style.backgroundColor = "green";
    //document.getElementById(otherID1).style.backgroundColor = "white";
    //document.getElementById(otherID2).style.backgroundColor = "white";
    //document.getElementById(otherID3).style.backgroundColor = "white";

    selectedAnswer = document.getElementById(selectedID).value;
}

function Reset()
{
//    document.getElementById("Meaning").style.backgroundColor = "white";
//    document.getElementById("Ans1").style.backgroundColor = "white";
//    document.getElementById("Ans2").style.backgroundColor = "white";
//    document.getElementById("Ans3").style.backgroundColor = "white";
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
        if (json.items[i].score === 4)
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