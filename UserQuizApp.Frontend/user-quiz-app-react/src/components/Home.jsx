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
      .then((json) => dispatch({ type: "UPDATE_LIST", payload: json }));
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
        .then((json) => dispatch({ type: "UPDATE_LIST", payload: json }));
    }
  }, []);

  const logOut = () => {
    localStorage.clear();
    navigate("/");
  };

  return (
    <div className="w-full flex flex-col items-center justify-center h-screen">
      <h1 className="text-green-400 text-xl font-bold mb-5">Select Quiz</h1>

      <div className="flex flex-col">
        {list?.map((item) => (
          <li
            key={item?.id}
            className="grid grid-cols-3 items-center w-96 justify-between"
          >
            <p className="text-lg ">{item?.quizName}</p>
            {item?.isCompleted ? (
              <p className="text-green-400">Completed</p>
            ) : (
              <p className="text-grey-400">Not started</p>
            )}

            <button
              onClick={() => navigate(`/quiz/${item?.id}`)}
              className="text-center bg-green-400 w-32 border border-gray-400 rounded-md h-5 text-sm font-bold"
            >
              Go to Quiz
            </button>
          </li>
        ))}
      </div>
      <button
        onClick={() => logOut()}
        className="text-center bg-green-400 text-base w-32 my-5 border border-gray-400 rounded-md h-10  font-bold"
      >
        LogOut
      </button>
    </div>
  );
};

export default Home;
