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
      `https://localhost:44348/Login?name=${user.name}&password=${user.password}`,
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
    <div className="w-full flex flex-col items-center justify-center h-screen">
      <h1 className="text-green-400 text-xl font-bold">Login</h1>
      <form
        onSubmit={handleSubmit}
        className="flex flex-col items-center justify-center"
      >
        <input
          className="w-32 border border-gray-400 rounded-md h-10 my-5 text-center"
          type="text"
          name="name"
          placeholder="Enter name"
          value={user.name}
          onChange={nameHandler}
        />

        <input
          className="w-32 border border-gray-400 rounded-md h-10 text-center"
          value={user.password}
          name="password"
          type={pass ? "text" : "password"}
          placeholder="Password"
          onChange={(e) => passwordHandler(e)}
        />

        <span onClick={togglePass} className=" text-gray-400 mt-2">
          Show password
        </span>

        <button
          type="submit"
          className="text-center bg-green-400  w-32 my-5 border border-gray-400 rounded-md h-10  font-bold"
        >
          Login
        </button>
      </form>
    </div>
  );
};

export default Login;
