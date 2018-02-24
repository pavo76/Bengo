function WriteFromJson(string)
{
    jsonString = '{"id":"1", "word":"おはよう", "answer":"Good Morning", "option1":"Good Day","option2":"Good Night","option3":"Good Evening"}';
    var object = JSON.parse(jsonString);
    document.getElementById("Word").innerHTML = object.word;
    document.getElementById("Meaning").innerHTML = object.answer;
    document.getElementById("Ans1").innerHTML = object.option1;
    document.getElementById("Ans2").innerHTML = object.option2;
    document.getElementById("Ans3").innerHTML = object.option3;
}