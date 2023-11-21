import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useParams, useNavigate } from "react-router-dom";

const Quiz = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const quizList = useSelector((state) => state.quizList);
  const { id } = useParams();
  const [score, setScore] = useState(0);
  const apiUrl = `https://localhost:44348`;
  const quizUrl = apiUrl + `/id?id=` + id;
  const headers = new Headers({
    Accept:
      "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7",
    "Accept-Encoding": "gzip, deflate, br",
    "Accept-Language": "uk,en-US;q=0.9,en;q=0.8",
    "Cache-Control": "max-age=0",
    Connection: "keep-alive",
    Authorization: "bearer " + localStorage.getItem("bearer"),
  });

  const handleCheckboxChange = (question, answer) => {
    const newAnswers = [...userAnswers];
    newAnswers[question] =
      quizList[question]?.questionAnswers[
        quizList[question]?.questionAnswers?.findIndex((x) => x.id === answer)
      ]?.isCorrect;
    setUserAnswers(newAnswers);
  };

  useEffect(() => {
    fetch(quizUrl, {
      method: "GET",
      headers: headers,
    })
      .then((res) => res.json())
      .then((json) => {
        dispatch({ type: "UPDATE_QUIZLIST", payload: json.list });
      });
  }, [id]);

  const [userAnswers, setUserAnswers] = useState(
    Array(quizList?.length).fill(false)
  );

  const handleSubmit = () => {
    let points = 0;
    for (let i = 0; i < userAnswers.length; i++) {
      const selected = userAnswers[i];
      const actual = true;
      if (selected === actual) points++;
    }
    setScore(points);
    navigate(`/results/${id}`);
  };

  return (
    <div className="w-full flex flex-col items-center justify-center h-screen">
      <h2 className="text-green-400 text-xl font-bold mb-5">Quiz</h2>

      <div className=" flex flex-col w-96">
        {quizList?.map((item) => (
          <li className="w-full flex flex-col">
            <p className="font-bold">{item?.questionWrapText}</p>
            {item?.questionAnswers.map((answer) => (
              <div className="w-full flex flex-row">
                <input
                  className="mr-2"
                  type="checkbox"
                  checked={userAnswers[quizList?.indexOf(item) === answer?.id]}
                  onChange={() =>
                    handleCheckboxChange(quizList?.indexOf(item), answer?.id)
                  }
                />
                <p>{answer.answerText}</p>
              </div>
            ))}
          </li>
        ))}

        <button
          onClick={() => handleSubmit()}
          className="text-center bg-green-400  w-32 my-5 border border-gray-400 rounded-md h-10  font-bold"
        >
          Submit
        </button>

        <div className="flex flex-row w-full justify-between">
          {
            <button
              onClick={() => navigate(`/home`)}
              className="text-center bg-green-400  w-32 my-5 border border-gray-400 rounded-md h-10  font-bold"
            >
              Home
            </button>
          }

          {
            <button
              onClick={() => navigate(`/`)}
              className="text-center bg-green-400  w-32 my-5 border border-gray-400 rounded-md h-10  font-bold"
            >
              Log Out
            </button>
          }
        </div>
      </div>
    </div>
  );
};

export default Quiz;
