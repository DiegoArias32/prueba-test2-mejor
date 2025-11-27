/fullName = clientEntity.FullName/a\
                },\
                branch = appointment.Branch != null ? new\
                {\
                    id = appointment.Branch.Id,\
                    name = appointment.Branch.Name,\
                    address = appointment.Branch.Address,\
                    phone = appointment.Branch.Phone,\
                    city = appointment.Branch.City\
                } : null,\
                appointmentType = appointment.AppointmentType != null ? new\
                {\
                    id = appointment.AppointmentType.Id,\
                    name = appointment.AppointmentType.Name,\
                    description = appointment.AppointmentType.Description,\
                    code = appointment.AppointmentType.Code
