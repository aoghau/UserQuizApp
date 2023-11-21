import { useNavigate, useParams } from "react-router-dom";

const Results = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const apiUrl = `https://localhost:44348`;
  const passUrl = apiUrl + `/pass?id=` + id;

  const headers = new Headers({
    Accept:
      "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7",
    "Accept-Encoding": "gzip, deflate, br",
    "Accept-Language": "uk,en-US;q=0.9,en;q=0.8",
    "Cache-Control": "max-age=0",
    Connection: "keep-alive",
    Authorization: "bearer " + localStorage.getItem("bearer"),
  });

  const passQuiz = () => {
    fetch(passUrl, { method: "POST", headers: headers }).then(
      navigate(`/home`)
    );
  };

  return (
    <div className="w-full flex flex-col items-center justify-center h-screen">
      <h4 className="text-green-400 text-xl font-bold mb-5">Submit Quiz?</h4>

      <button
        onClick={passQuiz}
        className="text-center bg-green-400 text-base w-32 my-5 border border-gray-400 rounded-md h-10  font-bold"
      >
        Yes
      </button>

      <button
        onClick={navigate(`/home`)}
        className="text-center bg-green-400 text-base w-32 my-5 border border-gray-400 rounded-md h-10  font-bold"
      >
        No
      </button>
    </div>
  );
};

export default Results;
