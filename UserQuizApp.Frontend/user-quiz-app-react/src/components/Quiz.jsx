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
        console.log(json.list);
      })
      .then(() => console.log(quizList));
  }, [id]);

  const [userAnswers, setUserAnswers] = useState(
    Array(quizList?.length).fill(false)
  );

  const handleSubmit = () => {
    let points = 0;
    for (let i = 0; i < userAnswers.length; i++) {
      const selected = userAnswers[i];
      console.log(selected);
      const actual = true;
      if (selected === actual) points++;
    }
    setScore(points);
    console.log(points);
    navigate(`/results/${id}`);
  };

  return (
    <div>
      <div>
        <h2>Quiz</h2>
      </div>
      <div>
        <ul>
          {quizList?.map((item) => (
            <li>
              <p>{item?.questionWrapText}</p>
              {item?.questionAnswers.map((answer) => (
                <div>
                  <input
                    type="checkbox"
                    checked={
                      userAnswers[quizList?.indexOf(item) === answer?.id]
                    }
                    onChange={() =>
                      handleCheckboxChange(quizList?.indexOf(item), answer?.id)
                    }
                  />
                  <p>{answer.answerText}</p>
                </div>
              ))}
            </li>
          ))}
        </ul>
        <div>
          <button onClick={() => handleSubmit()}>Submit</button>
        </div>
        <div>
          <h4>Results: {score}</h4>
        </div>
        <div>{/* <button onClick={navigate(`/`)}>Home</button> */}</div>
        <div>{/* <button onClick={navigate(`/login`)}>Log Out</button> */}</div>
      </div>
    </div>
  );
};

export default Quiz;
