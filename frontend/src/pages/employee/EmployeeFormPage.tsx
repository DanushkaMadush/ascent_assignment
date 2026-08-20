import {
  Alert,
  Box,
  Button,
  CircularProgress,
  MenuItem,
  Paper,
  Snackbar,
  TextField,
  Typography,
} from "@mui/material";

import { ArrowBack, Save } from "@mui/icons-material";

import { useState } from "react";
import { useNavigate } from "react-router-dom";

import { employeeService } from "../../api/services/employeeService";
import type { CreateEmployeeRequest } from "../../api/models/employee";

const departments = [
  { id: 1, name: "IT" },
  { id: 2, name: "Human Resources" },
  { id: 3, name: "Finance" },
  { id: 4, name: "Sales" },
];

const managers = [
  { id: "73cdb063-246d-454a-a4b3-c2288e341664", name: "HR Manager" },
  { id: "7d025651-f11c-483a-a72b-adeafaaeeece", name: "Administration Manager" },
  { id: "f73280ca-182f-4b8a-bd31-3fb8c8d70e03", name: "IT Manager" },
];

const roles = [
  "Admin",
  "Manager",
  "Employee",
];

const initialForm: CreateEmployeeRequest = {
  employeeCode: "",
  firstName: "",
  lastName: "",
  email: "",
  phoneNumber: "",
  password: "",
  departmentId: 0,
  managerId: "",
  role: "",
};

const EmployeeFormPage = () => {
  const navigate = useNavigate();

  const [form, setForm] = useState<CreateEmployeeRequest>(initialForm);

  const [saving, setSaving] = useState(false);

  const [errors, setErrors] = useState<
    Partial<Record<keyof CreateEmployeeRequest, string>>
  >({});

  const [snackbar, setSnackbar] = useState({
    open: false,
    message: "",
    severity: "success" as "success" | "error",
  });

  const handleChange = <K extends keyof CreateEmployeeRequest>(
    field: K,
    value: CreateEmployeeRequest[K],
  ) => {
    setForm((previous) => ({
      ...previous,
      [field]: value,
    }));

    setErrors((previous) => ({
      ...previous,
      [field]: undefined,
    }));
  };

  const validate = () => {
    const nextErrors: Partial<Record<keyof CreateEmployeeRequest, string>> = {};

    if (!form.employeeCode.trim()) {
      nextErrors.employeeCode = "Employee code is required.";
    }

    if (!form.firstName.trim()) {
      nextErrors.firstName = "First name is required.";
    }

    if (!form.lastName.trim()) {
      nextErrors.lastName = "Last name is required.";
    }

    if (!form.email.trim()) {
      nextErrors.email = "Email is required.";
    }

    if (!form.phoneNumber.trim()) {
      nextErrors.phoneNumber = "Phone number is required.";
    }

    if (!form.password.trim()) {
      nextErrors.password = "Password is required.";
    }

    if (!form.departmentId) {
      nextErrors.departmentId = "Department is required.";
    }

    if (!form.role) {
      nextErrors.role = "Role is required.";
    }

    setErrors(nextErrors);

    return Object.keys(nextErrors).length === 0;
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    if (!validate()) {
      return;
    }

    try {
      setSaving(true);

      const response = await employeeService.create(form);

      if (!response.success) {
        throw new Error(response.message || "Failed to create employee.");
      }

      setSnackbar({
        open: true,
        message: response.message || "Employee created successfully.",
        severity: "success",
      });

      setTimeout(() => {
        navigate("/employees");
      }, 700);
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "Failed to create employee.";

      setSnackbar({
        open: true,
        message,
        severity: "error",
      });
    } finally {
      setSaving(false);
    }
  };

  return (
    <Box>
      {/* Header */}
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          gap: 2,
          mb: 3,
        }}
      >
        <Button
          startIcon={<ArrowBack />}
          onClick={() => navigate("/employees")}
        >
          Back
        </Button>

        <Typography variant="h4" sx={{ fontWeight: 600 }}>
          Add Employee
        </Typography>
      </Box>

      <Paper
        component="form"
        onSubmit={handleSubmit}
        variant="outlined"
        sx={{
          p: 3,
        }}
      >
        <Typography variant="h6" sx={{ mb: 3 }}>
          Employee Information
        </Typography>

        <Box
          sx={{
            display: "grid",
            gridTemplateColumns: {
              xs: "1fr",
              sm: "1fr 1fr",
            },
            gap: 2,
          }}
        >
          <TextField
            label="Employee Code"
            value={form.employeeCode}
            onChange={(event) =>
              handleChange("employeeCode", event.target.value)
            }
            error={Boolean(errors.employeeCode)}
            helperText={errors.employeeCode}
            fullWidth
          />

          <TextField
            label="First Name"
            value={form.firstName}
            onChange={(event) => handleChange("firstName", event.target.value)}
            error={Boolean(errors.firstName)}
            helperText={errors.firstName}
            fullWidth
          />

          <TextField
            label="Last Name"
            value={form.lastName}
            onChange={(event) => handleChange("lastName", event.target.value)}
            error={Boolean(errors.lastName)}
            helperText={errors.lastName}
            fullWidth
          />

          <TextField
            label="Email"
            type="email"
            value={form.email}
            onChange={(event) => handleChange("email", event.target.value)}
            error={Boolean(errors.email)}
            helperText={errors.email}
            fullWidth
          />

          <TextField
            label="Phone Number"
            value={form.phoneNumber}
            onChange={(event) =>
              handleChange("phoneNumber", event.target.value)
            }
            error={Boolean(errors.phoneNumber)}
            helperText={errors.phoneNumber}
            fullWidth
          />

          <TextField
            label="Password"
            type="password"
            value={form.password}
            onChange={(event) => handleChange("password", event.target.value)}
            error={Boolean(errors.password)}
            helperText={errors.password}
            fullWidth
          />

          <TextField
            select
            label="Department"
            value={form.departmentId || ""}
            onChange={(event) =>
              handleChange("departmentId", Number(event.target.value))
            }
            error={Boolean(errors.departmentId)}
            helperText={errors.departmentId}
            fullWidth
          >
            {departments.map((department) => (
              <MenuItem key={department.id} value={department.id}>
                {department.name}
              </MenuItem>
            ))}
          </TextField>

          <TextField
            select
            label="Manager"
            value={form.managerId}
            onChange={(event) => handleChange("managerId", event.target.value)}
            error={Boolean(errors.managerId)}
            helperText={errors.managerId}
            fullWidth
          >
            {managers.map((manager) => (
              <MenuItem key={manager.id} value={manager.id}>
                {manager.name}
              </MenuItem>
            ))}
          </TextField>

          <TextField
            select
            label="Role"
            value={form.role}
            onChange={(event) => handleChange("role", event.target.value)}
            error={Boolean(errors.role)}
            helperText={errors.role}
            fullWidth
          >
            {roles.map((role) => (
              <MenuItem key={role} value={role}>
                {role}
              </MenuItem>
            ))}
          </TextField>
        </Box>

        <Box
          sx={{
            display: "flex",
            justifyContent: "flex-end",
            gap: 2,
            mt: 4,
          }}
        >
          <Button
            variant="outlined"
            onClick={() => navigate("/employees")}
            disabled={saving}
          >
            Cancel
          </Button>

          <Button
            type="submit"
            variant="contained"
            startIcon={saving ? <CircularProgress size={18} /> : <Save />}
            disabled={saving}
          >
            {saving ? "Creating..." : "Create Employee"}
          </Button>
        </Box>
      </Paper>

      <Snackbar
        open={snackbar.open}
        autoHideDuration={4000}
        onClose={() =>
          setSnackbar((previous) => ({
            ...previous,
            open: false,
          }))
        }
        anchorOrigin={{
          vertical: "bottom",
          horizontal: "right",
        }}
      >
        <Alert severity={snackbar.severity} variant="filled">
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
};

export default EmployeeFormPage;
