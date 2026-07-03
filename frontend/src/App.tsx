import { BrowserRouter, Routes, Route } from "react-router-dom";
import { ApplicationsPage } from "@/pages/ApplicationsPage";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<ApplicationsPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
