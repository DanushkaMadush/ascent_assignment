import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { AuthGuard } from "./AuthGuard";
import LoginPage from "../pages/auth/LoginPage";
import UnauthorizedPage from "../pages/auth/UnauthorizedPage";
import NotFoundPage from "../pages/auth/NotFoundPage";
import EmployeePage from "../pages/employee/EmployeePage";
import DepartmentPage from "../pages/department/DepartmentPage";
import AttendancePage from "../pages/attendance/AttendancePage";
import LeavePage from "../pages/leave/LeavePage";
import HomePage from "../pages/home/HomePage";
import EmployeeFormPage from "../pages/employee/EmployeeFormPage";

const AppRoutes = () => {
  return (
    <BrowserRouter>
      <Routes>
        
        <Route path="/" element={<Navigate to="/login" replace/>} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/unauthorized" element={<UnauthorizedPage />} />
        <Route path="/home" element={<HomePage />} />

        <Route element={<AuthGuard allowedRoles={["Employee", "Manager"]} />}>
          <Route path="/attendance" element={<AttendancePage />} />
          <Route path="/leaves" element={<LeavePage />} />
        </Route>

        <Route element={<AuthGuard allowedRoles={["Manager"]} />}>
          <Route path="/employees" element={<EmployeePage />} />
          <Route path="/employees/new" element={<EmployeeFormPage />} />
        </Route>

        <Route element={<AuthGuard allowedRoles={["Admin"]} />}>
          <Route path="/departments" element={<DepartmentPage />} />
        </Route>

        <Route path="*" element={<NotFoundPage />} />

      </Routes>
    </BrowserRouter>
  );
};

export default AppRoutes;
