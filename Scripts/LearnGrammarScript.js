var json;
var selectedAnswer;
//Set lowest score item index
var nextItemIndex=0;


function Start(jsonParameter)
{
    json = jsonParameter;
    document.getElementById("Word").innerHTML = json.items[0].word;
    RandomlySetAnswers(json.items[0].meaning, json.items[0].option1, json.items[0].option2, json.items[0].option3);
    document.getElementById("Score").innerHTML = "0/" + json.items.length;

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
        json.items[nextItemIndex].score = 1;
        answerIndicator.style.backgroundColor = "yellowgreen";
        answerIndicator.innerHTML = "Correct!"
        answerIndicator.style.display = "block";

    }
    else
    {       
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
            document.getElementById("Score").innerHTML = score + "/" + json.items.length;
        }
    }, 2500);
    
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
        if (json.items[i].score === 1)
        {
            result = true;
        }
        else if (json.items[i].score !== 1)
        {
            result = false;
            break;
        }
    }
    return result;
}