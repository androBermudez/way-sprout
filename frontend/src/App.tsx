import { BrowserRouter, Routes, Route } from "react-router-dom";
import { ApplicationsPage } from "@/pages/ApplicationsPage";
import { ApplicationDetailsPage } from "@/pages/ApplicationDetailsPage";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/applications" element={<ApplicationsPage />} />
        <Route path="/applications/:id" element={<ApplicationDetailsPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
