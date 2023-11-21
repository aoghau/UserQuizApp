import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";

const Home = () => {
  const navigate = useNavigate();
  const list = useSelector((state) => state.list);
  const wrapName = useSelector((state) => state.wrapName);
  const dispatch = useDispatch();
  const apiUrl = `https://localhost:44348`;
  const home = apiUrl + `/home`;

  const headers = new Headers({
    Accept:
      "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7",
    "Accept-Encoding": "gzip, deflate, br",
    "Accept-Language": "uk,en-US;q=0.9,en;q=0.8",
    "Cache-Control": "max-age=0",
    Connection: "keep-alive",
    Authorization: "bearer " + localStorage.getItem("bearer"),
  });

  const noAuthHeaders = new Headers({
    Accept: "application/json",
    "Access-Control-Allow-Origin": "*",
    "X-Requested-With": "XMLHttpRequest",
    "Access-Control-Allow-Methods": "GET,POST,PUT,DELETE,OPTIONS",
    "Access-Control-Allow-Headers":
      "Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With",
    "Content-Type": "application/json",
  });

  const getList = () => {
    fetch(home, {
      method: "GET",
      headers: headers,
    })
      .then((res) => res.json())
      .then((json) => dispatch({ type: "UPDATE_DATA", payload: json }));
  };

  const bearerToken = localStorage.getItem("bearer");

  useEffect(() => {
    if (bearerToken) getList();
    else {
      fetch(home, {
        method: "GET",
        headers: noAuthHeaders,
      })
        .then((res) => res.json())
        .then((json) => dispatch({ type: "UPDATE_DATA", payload: json }));
    }
  }, []);

  const logOut = () => {
    localStorage.clear();
    navigate("/");
  };

  return (
    <div>
      <div>
        <h1 className="text-green-400">Select Quiz</h1>
      </div>
      <div>
        <ul>
          {list?.map((item) => (
            <li key={item?.id}>
              <p>{item?.quizName}</p>
              <p
                style={{
                  color: item?.isCompleted ? "text-green-400" : "text-grey-400",
                }}
              >
                {item.isCompleted ? "Completed" : "Not started"}
              </p>
              <button onClick={() => navigate("/")}>Start</button>
              <button onClick={() => navigate(`/quiz/${item?.id}`)}>
                Go to Quiz
              </button>
            </li>
          ))}
        </ul>
      </div>
      <button onClick={() => logOut()}>LogOut</button>
    </div>
  );
};

export default Home;
