import { useState } from "react";
import { useNavigate } from "react-router-dom";

const Login = () => {
  const [pass, setPass] = useState(false);
  const [error, setError] = useState();
  const [user, setUser] = useState({
    name: "",
    password: "",
  });
  const navigate = useNavigate();

  const togglePass = () => {
    setPass((prevState) => !prevState);
  };

  const nameHandler = (e) => {
    setUser({
      ...user,
      [e.target.name]: [e.target.value],
    });
  };

  const passwordHandler = (e) => {
    setUser({
      ...user,
      [e.target.name]: [e.target.value],
    });
    if (e.target.value.length < 8)
      setError("Username or password are invalid.");
    else setError();
  };

  const saveUser = () => {
    fetch(
      `http://localhost:7268/Login?name=${user.name}&password=${user.password}`,
      {
        method: "POST",
        headers: {
          Accept: "application/json",
          "Access-Control-Allow-Origin": "*",
          "X-Requested-With": "XMLHttpRequest",
          "Access-Control-Allow-Methods": "GET,POST,PUT,DELETE,OPTIONS",
          "Access-Control-Allow-Headers":
            "Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With",
          "Content-Type": "application/json",
        },
      }
    )
      .then((res) => res.json())
      .then((json) => {
        localStorage.setItem("bearer", json);
        if (json) {
          navigate("/home");
        } else {
          setError("Username or password are invalid.");
        }
      })

      .catch((err) => console.log("error"));
  };

  function handleSubmit(e) {
    e.preventDefault();

    saveUser();
  }

  return (
    <div className="container logup__wrapper is-flex is-flex-direction-column">
      <h1 className="content logup__title has-text-centered is-3">Login</h1>
      <form onSubmit={handleSubmit}>
        <div className="field">
          <div className="control">
            <input
              className="input"
              type="text"
              name="name"
              placeholder="Enter name"
              value={user.name}
              onChange={nameHandler}
            />
          </div>
        </div>
        <div className="field">
          <div className="control">
            <input
              className="input"
              value={user.password}
              name="password"
              type={pass ? "text" : "password"}
              placeholder="Password"
              onChange={(e) => passwordHandler(e)}
            />
          </div>
        </div>
        <div className="logup__fild field control">
          <span onClick={togglePass} className="logup__text">
            Show password
          </span>
        </div>
        <button type="submit" className="button is-danger is-5 title btn">
          Login
        </button>
      </form>
    </div>
  );
};

export default Login;
