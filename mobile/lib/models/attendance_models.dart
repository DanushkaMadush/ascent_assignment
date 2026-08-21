class AttendanceResponse {
  final int id;
  final String employeeId;
  final String employeeCode;
  final String employeeName;
  final DateTime? checkInTime;
  final DateTime? checkOutTime;
  final DateTime? workDate;
  final String status;
  final String deviceType;

  AttendanceResponse({
    required this.id,
    required this.employeeId,
    required this.employeeCode,
    required this.employeeName,
    this.checkInTime,
    this.checkOutTime,
    this.workDate,
    required this.status,
    required this.deviceType,
  });

  factory AttendanceResponse.fromJson(Map<String, dynamic> json) {
    return AttendanceResponse(
      id: json['id'] ?? 0,
      employeeId: json['employeeId'] ?? '',
      employeeCode: json['employeeCode'] ?? '',
      employeeName: json['employeeName'] ?? '',
      checkInTime: json['checkInTime'] != null
          ? DateTime.tryParse(json['checkInTime'].toString())
          : null,
      checkOutTime: json['checkOutTime'] != null
          ? DateTime.tryParse(json['checkOutTime'].toString())
          : null,
      workDate: json['workDate'] != null
          ? DateTime.tryParse(json['workDate'].toString())
          : null,
      status: json['status'] ?? '',
      deviceType: json['deviceType'] ?? '',
    );
  }
}

class CheckInRequest {
  final String employeeId;
  final String deviceType;

  CheckInRequest({
    required this.employeeId,
    required this.deviceType,
  });

  Map<String, dynamic> toJson() {
    return {
      'employeeId': employeeId,
      'deviceType': deviceType,
    };
  }
}

class CheckOutRequest {
  final String employeeId;

  CheckOutRequest({
    required this.employeeId,
  });

  Map<String, dynamic> toJson() {
    return {
      'employeeId': employeeId,
    };
  }
}