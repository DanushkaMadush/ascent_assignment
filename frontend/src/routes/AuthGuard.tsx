import { Navigate, Outlet, useLocation } from "react-router-dom";

import { tokenStorage } from "../api/client/tokenStorage";
import { getRoleFromToken, type Role } from "../api/client/jwt";

interface AuthGuardProps {
  allowedRoles?: Role[];
}

export const AuthGuard = ({ allowedRoles }: AuthGuardProps) => {
  const location = useLocation();
  const accessToken = tokenStorage.getAccessToken();

  // Not logged in
  if (!accessToken) {
    return (
      <Navigate
        to="/login"
        replace
        state={{ from: location }}
      />
    );
  }

  const role = getRoleFromToken(accessToken);

  // No valid role
  if (!role) {
    return <Navigate to="/unauthorized" replace />;
  }

  // Admin can access everything
  if (role === "Admin") {
    return <Outlet />;
  }

  // Check allowed roles
  if (allowedRoles?.includes(role)) {
    return <Outlet />;
  }

  return <Navigate to="/unauthorized" replace />;
};