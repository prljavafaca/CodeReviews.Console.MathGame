//You need to create a Math game containing the 4 basic operations
//The divisions should result on INTEGERS ONLY and dividends should go from 0 to 100. Example: Your app shouldn't present the division 7/2 to the user, since it doesn't result in an integer.
//Users should be presented with a menu to choose an operation
//You should record previous games in a List and there should be an option in the menu for the user to visualize a history of previous games.
//You don't need to record results on a database. Once the program is closed the results will be deleted.



using System;
using System.Diagnostics;
using MathGame;

MathGameLogic mathGame = new();
Random random = new();

int firstNumber;
int secondNumber;
int userMenuSelection;
int score = 0;
bool gameOver = false;

DifficultyLevel difficulty = DifficultyLevel.Easy;

while(!gameOver)
{
    userMenuSelection = GetUserMenuSelection(mathGame);

    firstNumber = random.Next(1, 101);
    secondNumber = random.Next(1, 101);

    switch (userMenuSelection)
    {
        case 1:
            score = await PerformOperation(mathGame, firstNumber, secondNumber, score, difficulty, '+');
            break;
        case 2:
            score = await PerformOperation(mathGame, firstNumber, secondNumber, score, difficulty, '-');
            break;
        case 3:
            score = await PerformOperation(mathGame, firstNumber, secondNumber, score, difficulty, '*');
            break;
        case 4:
            while (firstNumber % secondNumber != 0) // needs to be an integer division
            {
                firstNumber = random.Next(1, 101);
                secondNumber = random.Next(1, 101);
            }
            score = await PerformOperation(mathGame, firstNumber, secondNumber, score, difficulty, '/');
            break;
        case 5:
            int numberOfQuestions = 99;
            Console.WriteLine("Please enter a number of questions you want to atempt.");
            while (!int.TryParse(Console.ReadLine(), out numberOfQuestions))
            {
                Console.WriteLine("Please enter a number of questions you want to atempt  as an INTEGER number.");
            }
            while (numberOfQuestions > 0)
            {
                int randomOperation = random.Next(1, 5);

                switch (randomOperation)
                {
                    case 1:
                        firstNumber = random.Next(1, 101);
                        secondNumber = random.Next(1, 101);

                        score = await PerformOperation(mathGame, firstNumber, secondNumber, score, difficulty, '+');
                        break;

                    case 2:

                        firstNumber = random.Next(1, 101);
                        secondNumber = random.Next(1, 101);

                        score = await PerformOperation(mathGame, firstNumber, secondNumber, score, difficulty, '-');
                        break;

                    case 3:

                        firstNumber = random.Next(1, 101);
                        secondNumber = random.Next(1, 101);

                        score = await PerformOperation(mathGame, firstNumber, secondNumber, score, difficulty, '*');
                        break;

                    case 4:
                        while (firstNumber % secondNumber == 0) // needs to be an integer division
                        {
                            firstNumber = random.Next(1, 101);
                            secondNumber = random.Next(1, 101);
                        }
                        score = await PerformOperation(mathGame, firstNumber, secondNumber, score, difficulty, '/');
                        break;
                }
                numberOfQuestions--;
            }
            break;
            case 6:
                Console.WriteLine("Game History: \n ");
                foreach (var operation in mathGame.GameHistory)
                {
                    Console.WriteLine($"{operation}");
                }
                break;

            case 7:
                difficulty = ChangeDifficulty();
                DifficultyLevel difficultyEnum = (DifficultyLevel)difficulty;

                Enum.IsDefined(typeof(DifficultyLevel), difficultyEnum);
                Console.WriteLine($"Your new difficulty level is {difficulty}");

                break;
            case 8:
                gameOver = true;
                Console.WriteLine($"Your final score is {score}");
                break;
    }
}



static DifficultyLevel ChangeDifficulty()
{
    int userSelection = 0;

    Console.WriteLine("Please select a difficulty level.");
    Console.WriteLine("1. Easy");
    Console.WriteLine("2. Medium");
    Console.WriteLine("3. Hard");

    while (!int.TryParse(Console.ReadLine(), out userSelection) || (userSelection < 1 || userSelection > 3))
    {
        Console.WriteLine("Please select a number from 1 to 3");
    }
    switch (userSelection)
    {
        case 1:
            return DifficultyLevel.Easy;
        case 2:
            return DifficultyLevel.Medium;
        case 3:
            return DifficultyLevel.Hard;
        default:
            return DifficultyLevel.Easy;
    }
}

static void DisplayMathQuestion(int firstNumber, int secondNumber, char operation)
{
    Console.WriteLine($"{firstNumber} {operation} {secondNumber} = ??");
}

static int GetUserMenuSelection(MathGameLogic mathGame)
{
    int selection = -1;
    mathGame.ShowMenu();
    while (selection < 1 || selection > 8)
    {
        while (!int.TryParse(Console.ReadLine(), out selection))
        {
            Console.WriteLine("Please enter a valid selection number 1-8.");
        }
    }
    return selection;
}

static async Task<int?> GetUserResponse(DifficultyLevel difficulty)
{
    int response = 0;
    int timeout = (int)difficulty;

    Stopwatch stopWatch = new();
    stopWatch.Start();

    Task<string?> getUserInputTask = Task.Run(() => Console.ReadLine());

    try
    {
        string? result = await Task.WhenAny(getUserInputTask, Task.Delay(timeout * 1000)) == getUserInputTask ? getUserInputTask.Result : null;

        stopWatch.Stop();

        if (result != null && int.TryParse(result, out response))
        {
            Console.WriteLine($"Time taken to answer: {stopWatch.Elapsed.ToString(@"m\:ss\.fff")}");
            return response;
        }

        else
        {
            throw new OperationCanceledException();
        }
    }

    catch (OperationCanceledException)
    {
        Console.WriteLine("Time is up!");
        return null;
    }
}

static int ValidateResult(int result, int? userResponse, int score)
{
    if (result == userResponse)
    {
        Console.WriteLine("Congratulations! Your answer is correct. You earned 5 points");
        score += 5;
    }
    else
    {
        Console.WriteLine("Try again!");
        Console.WriteLine($"Correct result is: {result}");
    }
    return score;
}

static async Task<int> PerformOperation(MathGameLogic mathGame, int firstNumber, int secondNumber, int score, DifficultyLevel difficulty, char operation)
{
    int result;
    int? userResponse;

    DisplayMathQuestion(firstNumber, secondNumber, operation);
    result = mathGame.MathOperation(firstNumber, secondNumber, operation);

    userResponse = await GetUserResponse(difficulty);
    score = ValidateResult(result, userResponse, score);

    return score;
}



public enum DifficultyLevel //Represents the amount of time a user will have depending on their difficulty level. Enums are a class containing read-only group of constants.
{
    Easy = 45,

    Medium = 30,

    Hard = 15,
}



