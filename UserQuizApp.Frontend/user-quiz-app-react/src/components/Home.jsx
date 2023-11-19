import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";

const Home = () => {
  const navigate = useNavigate();
  const list = useSelector((state) => state.list);
  const wrapName = useSelector((state) => state.wrapName);
  const dispatch = useDispatch();
  const apiUrl = `http://localhost:44348`;
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

  const getList = () => {
    fetch(home, {
      method: "GET",
      headers: headers,
    })
      .then((res) => res.json)
      .then((json) => dispatch({ type: "UPDATE_DATA", payload: json }));
  };

  const bearerToken = localStorage.getItem("bearer");

  useEffect(() => {
    if (bearerToken) getList();
    else {
      fetch(home, {
        method: "GET",
        headers: headers,
      })
        .then((res) => res.json)
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
        <h1>Select Quiz</h1>
      </div>
      <div>
        <ul>
          {list.map((item) => (
            <li key={item.Id}>
              <p>{item.QuizName}</p>
              <p style={{ color: item.IsCompleted ? "green" : "grey" }}>
                {item.IsCompleted ? "Completed" : "Not started"}
              </p>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
};

export default Home;
