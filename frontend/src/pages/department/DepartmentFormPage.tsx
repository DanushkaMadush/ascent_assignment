import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Paper,
  Snackbar,
  TextField,
  Typography,
} from "@mui/material";

import { ArrowBack, Save } from "@mui/icons-material";

import { useState } from "react";

import { useNavigate } from "react-router-dom";

import { departmentService } from "../../api/services/departmentService";

import type { CreateDepartmentRequest } from "../../api/models/department";

const initialForm: CreateDepartmentRequest = {
  name: "",
  description: "",
};

const DepartmentFormPage = () => {
  const navigate = useNavigate();

  const [form, setForm] = useState<CreateDepartmentRequest>(initialForm);

  const [saving, setSaving] = useState(false);

  const [errors, setErrors] = useState<
    Partial<Record<keyof CreateDepartmentRequest, string>>
  >({});

  const [snackbar, setSnackbar] = useState({
    open: false,
    message: "",
    severity: "success" as "success" | "error",
  });

  const handleChange = <K extends keyof CreateDepartmentRequest>(
    field: K,
    value: CreateDepartmentRequest[K],
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
    const nextErrors: Partial<Record<keyof CreateDepartmentRequest, string>> =
      {};

    if (!form.name.trim()) {
      nextErrors.name = "Department name is required.";
    }

    if (!form.description.trim()) {
      nextErrors.description = "Description is required.";
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

      const request: CreateDepartmentRequest = {
        name: form.name.trim(),
        description: form.description.trim(),
      };

      const response = await departmentService.create(request);

      if (!response.success) {
        throw new Error(response.message || "Failed to create department.");
      }

      setSnackbar({
        open: true,
        message: response.message || "Department created successfully.",
        severity: "success",
      });

      setTimeout(() => {
        navigate("/departments");
      }, 700);
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "Failed to create department.";

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
    <Box sx={{ margin: 15 }}>
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
          onClick={() => navigate("/departments")}
        >
          Back
        </Button>

        <Typography variant="h4" sx={{ fontWeight: 600 }}>
          Add Department
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
          Department Information
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
            label="Department Name"
            value={form.name}
            onChange={(event) => handleChange("name", event.target.value)}
            error={Boolean(errors.name)}
            helperText={errors.name}
            fullWidth
            required
          />

          <TextField
            label="Description"
            value={form.description}
            onChange={(event) =>
              handleChange("description", event.target.value)
            }
            error={Boolean(errors.description)}
            helperText={errors.description}
            fullWidth
            required
            multiline
            minRows={4}
            sx={{
              gridColumn: {
                xs: "auto",
                sm: "1 / -1",
              },
            }}
          />
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
            onClick={() => navigate("/departments")}
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
            {saving ? "Creating..." : "Create Department"}
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

export default DepartmentFormPage;
