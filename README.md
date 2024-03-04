# WPF를 이용한 윈도우 채팅프로그램
`C# WPF`와 `Postgresql`를 이용한 1:N 비동기 채팅 프로그램
## Postgresql 테이블 구조
| Name       | Data Type | Length | Not NULL | Primary Key | Unique |
|------------|-----------|--------|----------|-------------|--------|
| user_id    | integer   |        | TRUE     |○|
| username   | varchar   | 50     | TRUE     ||○|
| password   | varchar   | 50     | TRUE     ||
| created_on | timestamp |        | TRUE     ||


## 로그인 화면 (메인 폼)
![로그인화면(메인)](/assets/screenshot/로그인화면(메인).jpg)
![로그인성공)](/assets/screenshot/로그인성공메세지박스.jpg)

## 회원가입 화면
![회원가입화면](/assets/screenshot/회원가입화면.jpg)
![가입성공](/assets/screenshot/가입완료메세지박스.jpg)
- 이미 가입된 `username`으로 가입 불가능 (username칼럼이 `UNIQUE`)

## 로그인 성공 시 채팅 폼
![](/assets/screenshot/채팅방화면.jpg)
- 로그인된 계정의 `username`과 접속 컴퓨터의 `로컬IP`를 표시
- 다른 클라이언트의 접속 또는 종료 시 즉시 현재 접속자 리스트 업데이트

## 작동 영상
![시연영상](/assets/screenshot/시연.gif)
