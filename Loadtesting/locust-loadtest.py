from locust import HttpUser, TaskSet, task, between, events
import random
import string

class OpenPostsTask(TaskSet):

  def on_start(self):
    self.login()

  def login(self):
    response = self.client.post("/User/login", json={"id": "76561198051924392", "authToken": "test"})
    response_json = response.json()
    self.token = response_json["jwtToken"]

  @task(1)
  def fetch_posts(self):
    headers = {"Authorization": f"Bearer {self.token}"}
    response = self.client.get("/Comment/all/difficulty/be0e60eb-6b02-46fa-9239-110803e381b3", headers=headers)
    response2 = self.client.get("/Comment/all/mapset/502bd2ff-ea01-4594-a299-7b797e0e030f", headers=headers)
    response3 = self.client.get("/Comment/stats/502bd2ff-ea01-4594-a299-7b797e0e030f/be0e60eb-6b02-46fa-9239-110803e381b3", headers=headers)
    if response.status_code == 200 & response2.status_code == 200 & response3.status_code == 200:
      print("Posts recieved successfully!")
    else:
      print(f"Error recieving posts: {response.text}")

class OpenMapsetListTask(TaskSet):

  def on_start(self):
    self.login()

  def login(self):
    response = self.client.post("/User/login", json={"id": "76561198051924392", "authToken": "test"})
    response_json = response.json()
    self.token = response_json["jwtToken"]

  @task(1)
  def fetch_comments(self):
    headers = {"Authorization": f"Bearer {self.token}"}
    response = self.client.get("/MapDiscussion/all/phase0", headers=headers)
    if response.status_code == 200:
      print("Comments recieved successfully!")
    else:
      print(f"Error recieving comments: {response.text}")

class PostCommentTask(TaskSet):

  def on_start(self):
    self.login()

  def login(self):
    response = self.client.post("/User/login", json={"id": "76561198051924392", "authToken": "test"})
    response_json = response.json()
    self.token = response_json["jwtToken"]
    
  @task(1)
  def post_comment(self):
    headers = {"Authorization": f"Bearer {self.token}"}

    random_string = ''.join(random.choices(string.ascii_letters + string.digits, k=12))

    data = {
      "mapDiscussionId": "502bd2ff-ea01-4594-a299-7b797e0e030f",
      "difficultyDiscussionId": "be0e60eb-6b02-46fa-9239-110803e381b3",
      "body": "This is a load-test Comment of type Suggestion! LOADTEST-54632 RS:" + random_string,
      "type": 2
    }

    response = self.client.post("/Comment", json=data, headers=headers)
    if response.status_code == 201:
      print("Comment posted successfully!")
    else:
      print(f"Error posting comment: {response.text}")

class PostReplyTask(TaskSet):

  def on_start(self):
    self.login()

  def login(self):
    response = self.client.post("/User/login", json={"id": "76561198051924392", "authToken": "test"})
    response_json = response.json()
    self.token = response_json["jwtToken"]

  @task(1)
  def post_post(self):
    headers = {"Authorization": f"Bearer {self.token}"}

    random_string = ''.join(random.choices(string.ascii_letters + string.digits, k=12))

    data = {
      "commentId": "bc529f2d-95e2-4da5-a638-1c53717aac64 ",
      "body": "This is a load-test reply! LOADTEST-54632 RS:" + random_string
    }

    response = self.client.post("/Reply", json=data, headers=headers)
    if response.status_code == 200:
      print("Reply posted successfully!")
    else:
      print(f"Error posting reply: {response.text}")

class WebsiteLurkUser(HttpUser):
  tasks = [OpenPostsTask]
  wait_time = between(1,5)
  weight = 100

class WebsiteLurkUser2(HttpUser):
  tasks = [OpenMapsetListTask]
  wait_time = between(1,5)
  weight = 100

class WebsitePostUser(HttpUser):
  tasks = [PostReplyTask]
  wait_time = between(5,10)
  weight = 10

class WebsiteCommentUser(HttpUser):
  tasks = [PostCommentTask]
  wait_time = between(5,10)
  weight = 20

def on_test_stop(environment, **kwargs):
    with WebsiteLurkUser(environment) as user:
        user.client.post("/Comment/LoadtestCleanup")
        user.client.post("/Reply/LoadtestCleanup")

events.test_stop.add_listener(on_test_stop)